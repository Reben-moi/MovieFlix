using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieFlix.Data;
using MovieFlix.Models;
using MovieFlix.Services;
using MovieFlix.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MovieFlix.Controllers
{
    [Authorize(Roles = "SuperAdmin,CinemaAdmin")]
    public class MovieController : Controller
    {
        private readonly MovieflixContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuditService _auditService;



        public MovieController(MovieflixContext context, UserManager<ApplicationUser> userManager, AuditService auditService)
        {
            _context = context;
            _userManager = userManager;
            _auditService = auditService;
        }


        public async Task<IActionResult> Index(string genreId, string? searchQuery)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var query = _context.Movies.Include(m => m.Genre).AsQueryable();

            if (User.IsInRole("CinemaAdmin") && currentUser?.CinemaId != null)
            {
                query = query.Where(m => m.CinemaId == currentUser.CinemaId);
            }

            if (!string.IsNullOrEmpty(genreId) && int.TryParse(genreId, out int selectedGenreId))
            {
                query = query.Where(m => m.GenreId == selectedGenreId);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var lowerSearchQuery = searchQuery.ToLower();
                query = query.Where(m => m.Title.ToLower().Contains(lowerSearchQuery));
            }

            var movies = await query.ToListAsync();
            return View(movies);
        }


        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var cinemasQuery = _context.Cinemas.AsQueryable();
            if (User.IsInRole("CinemaAdmin") && currentUser?.CinemaId != null)
            {
                cinemasQuery = cinemasQuery.Where(c => c.Id == currentUser.CinemaId);
            }

            var viewModel = new MovieFormViewModel
            {
                Genres = await _context.Genres
                    .Select(g => new SelectListItem { Value = g.GenreId.ToString(), Text = g.Name })
                    .ToListAsync(),
                Cinemas = await cinemasQuery
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToListAsync()
            };

            return View(viewModel);
        }



        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieFormViewModel viewModel)
        {
            if (User.IsInRole("CinemaAdmin"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                viewModel.CinemaId = currentUser?.CinemaId ?? viewModel.CinemaId;
            }

            ModelState.Remove(nameof(viewModel.Cinemas));

            if (!ModelState.IsValid)
            {
            
                viewModel.Genres = await _context.Genres.Select(g => new SelectListItem
                {
                    Value = g.GenreId.ToString(),
                    Text = g.Name
                }).ToListAsync();

                viewModel.Cinemas = await _context.Cinemas.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToListAsync();

                return View(viewModel);
            }

            var movie = new Movie
            {
                Title = viewModel.Title,
                GenreId = viewModel.GenreId,
                ReleaseDate = viewModel.ReleaseDate,
                Description = viewModel.Description,
                Rating = viewModel.Rating,
                CinemaId = viewModel.CinemaId, // 👈 Save selected cinema
            };

            if (viewModel.ImageFile != null && viewModel.ImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(viewModel.ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.ImageFile.CopyToAsync(stream);
                }

                movie.ImageUrl = "/images/" + fileName;
            }

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                action: "Created",
                entityName: "Movie",
                entityId: movie.MovieId,
                details: $"Title: {movie.Title}, GenreId: {movie.GenreId}, CinemaId: {movie.CinemaId}"
            );
            return RedirectToAction(nameof(Index));
        }


        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {



            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("CinemaAdmin") && movie.CinemaId != currentUser?.CinemaId)
            {
                return Forbid();
            }

            var viewModel = new MovieFormViewModel
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                ReleaseDate = movie.ReleaseDate,
                GenreId = movie.GenreId,
                ImageUrl = movie.ImageUrl,
                Description = movie.Description,
                Rating = movie.Rating,
                CinemaId = movie.CinemaId,
                Genres = await _context.Genres.Select(g => new SelectListItem
                {
                    Value = g.GenreId.ToString(),
                    Text = g.Name
                }).ToListAsync(),
                Cinemas = await _context.Cinemas.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToListAsync()

            };

            return View(viewModel);
        }


        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MovieFormViewModel viewModel)
        {
            

            ModelState.Remove(nameof(viewModel.Cinemas));
            if (id != viewModel.MovieId) return NotFound();

            if (!ModelState.IsValid)
            {
                viewModel.Genres = _context.Genres.Select(g => new SelectListItem
                {
                    Value = g.GenreId.ToString(),
                    Text = g.Name
                }).ToList();

                viewModel.Cinemas = _context.Cinemas.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

                return View(viewModel); // return with dropdowns populated
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("CinemaAdmin") && movie.CinemaId != currentUser?.CinemaId)
            {
                return Forbid();
            }

            movie.Title = viewModel.Title;
            movie.ReleaseDate = viewModel.ReleaseDate;
            movie.GenreId = viewModel.GenreId;
            movie.Description = viewModel.Description;
            movie.Rating = viewModel.Rating;
            movie.CinemaId = viewModel.CinemaId;

            if (viewModel.ImageFile != null && viewModel.ImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(viewModel.ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.ImageFile.CopyToAsync(stream);
                }

                movie.ImageUrl = "/images/" + fileName;
            }

            try
            {
                _context.Update(movie);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Movies.Any(e => e.MovieId == id)) return NotFound();
                throw;
            }

            await _auditService.LogAsync(
                action: "Edited",
                entityName: "Movie",
                entityId: movie.MovieId,
                details: $"Updated movie Title to: {movie.Title}, GenreId: {movie.GenreId}, CinemaId: {movie.CinemaId}"
            );

            return RedirectToAction(nameof(Index));
        }


        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            

            if (id == null) return NotFound();

            var movie = await _context.Movies
                .Include(movie => movie.Genre)
                .FirstOrDefaultAsync(movie => movie.MovieId == id);

            if (movie == null) return NotFound();
            var currentUser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("CinemaAdmin") && movie.CinemaId != currentUser?.CinemaId)
            {
                return Forbid();
            }
            return View(movie);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();
            var currentUser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("CinemaAdmin") && movie.CinemaId != currentUser?.CinemaId)
            {
                return Forbid();
            }

           
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();

                await _auditService.LogAsync(
                    action: "Deleted",
                    entityName: "Movie",
                    entityId: movie.MovieId,
                    details: $"Deleted movie Title: {movie.Title}"
                );
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
