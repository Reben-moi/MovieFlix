using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieFlix.Data;
using MovieFlix.Models;
using MovieFlix.ViewModels;
using System.Diagnostics;

namespace MovieFlix.Controllers
{
    public class HomeController : Controller
    {
        private readonly MovieflixContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(MovieflixContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        public async Task<IActionResult> Index(int? genreId, int? cinemaId, string? searchQuery)
        {
            var currentUser = await _userManager.GetUserAsync(User);
           
            var moviesQuery = _context.Movies.Include(m => m.Genre).Include(m => m.Cinema).AsQueryable();

            if (User.IsInRole("CinemaAdmin") && currentUser?.CinemaId != null)
            {
                moviesQuery = moviesQuery.Where(m => m.CinemaId == currentUser.CinemaId);
            }

           

            if (genreId.HasValue)
                moviesQuery = moviesQuery.Where(m => m.GenreId == genreId);

            if (cinemaId.HasValue)
                moviesQuery = moviesQuery.Where(m => m.CinemaId == cinemaId);

            if (!string.IsNullOrEmpty(searchQuery))
                moviesQuery = moviesQuery.Where(m => m.Title.Contains(searchQuery));

            var viewModel = new HomeIndexViewModel
            {
                Movies = await moviesQuery.ToListAsync(),
                Genres = await _context.Genres.Select(g => new SelectListItem
                {
                    Text = g.Name,
                    Value = g.GenreId.ToString()
                }).ToListAsync(),
                Cinemas = await _context.Cinemas.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }).ToListAsync(),
                SelectedGenreId = genreId,
                SelectedCinemaId = cinemaId,
                SearchQuery = searchQuery
            };

            return View(viewModel);
        }






        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
