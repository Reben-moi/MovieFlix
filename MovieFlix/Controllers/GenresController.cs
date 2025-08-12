using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieFlix.Data;
using MovieFlix.Models;
using MovieFlix.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieFlix.Controllers
{
    public class GenresController : Controller
    {
        private readonly MovieflixContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GenresController(MovieflixContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Genres
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            IQueryable<Genre> genresQuery = _context.Genres.Include(g => g.Movies);


            if (User.IsInRole("CinemaAdmin") && currentUser?.CinemaId != null)
            {
             
            }

            var genres = await genresQuery.ToListAsync();


            var viewModelList = genres.Select(g => new GenreViewModel
            {
                GenreId = g.GenreId,
                Name = g.Name,
                MovieCount = User.IsInRole("CinemaAdmin") && currentUser?.CinemaId != null
                    ? g.Movies.Count(m => m.CinemaId == currentUser.CinemaId)
                    : g.Movies.Count
            }).ToList();

            return View(viewModelList);
        }



        // GET: Genres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres
                .FirstOrDefaultAsync(m => m.GenreId == id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // GET: Genres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Genres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GenreId,Name")] Genre genre)
        {
            if (ModelState.IsValid)
            {
                _context.Add(genre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(genre);
        }

        // GET: Genres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }
            return View(genre);
        }

        // POST: Genres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GenreId,Name")] Genre genre)
        {
            if (id != genre.GenreId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GenreExists(genre.GenreId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(genre);
        }

        // GET: Genres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres
                .FirstOrDefaultAsync(m => m.GenreId == id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // POST: Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GenreExists(int id)
        {
            return _context.Genres.Any(e => e.GenreId == id);
        }

        [HttpGet]
        public async Task<IActionResult> GetGenres()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            IQueryable<Genre> genresQuery = _context.Genres.Include(g => g.Movies);

            var genres = await genresQuery.ToListAsync();

            var isCinemaAdmin = User.IsInRole("CinemaAdmin");
            var userCinemaId = currentUser?.CinemaId;

            var genreViewModels = genres.Select(g => new GenreViewModel
            {
                GenreId = g.GenreId,
                Name = g.Name,
                // If CinemaAdmin, count movies belonging only to user's CinemaId
                MovieCount = (isCinemaAdmin && userCinemaId != null)
                    ? g.Movies.Count(m => m.CinemaId == userCinemaId)
                    : g.Movies.Count
            }).ToList();

            return Json(new { data = genreViewModels });
        }


    }
}
