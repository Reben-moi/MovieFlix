using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieFlix.Data;
using MovieFlix.Models;
using MovieFlix.Models.Dtos;
using MovieFlix.ViewModels;
using System.Linq;
using System.Threading.Tasks;


[Authorize]
public class AnalyticsController : Controller
{
    private readonly MovieflixContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AnalyticsController(MovieflixContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordHover([FromBody] HoverDataDto data)
    {
        var user = await _userManager.GetUserAsync(User);
        var movie = await _context.Movies.FindAsync(data.MovieId);
        if (movie == null) return NotFound();
        if (user == null)
        {
            return Unauthorized(); // or handle as needed
        }

        var analytics = new MovieAnalytics
        {
            MovieId = data.MovieId,
            UserId = user.Id,
            HoverDurationMs = data.HoverDuration,
            CinemaId = movie.CinemaId ?? 0
        };

        _context.MovieAnalytics.Add(analytics);
        await _context.SaveChangesAsync();

        return Ok();
    }
    [Authorize(Roles = "CinemaAdmin,SuperAdmin")]
    public async Task<IActionResult> CinemaAnalytics(int? cinemaId)
    {
        var user = await _userManager.GetUserAsync(User);

        if (User.IsInRole("CinemaAdmin"))
        {
            cinemaId = user?.CinemaId;
        }

        // Get popular movies
        var popularMovies = await _context.MovieAnalytics
       .Where(a => cinemaId == null || a.CinemaId == cinemaId)
       .Join(
           _context.Movies,
           analytics => analytics.MovieId,
           movie => movie.MovieId,
           (analytics, movie) => new { analytics, movie }
       )
       .GroupBy(x => x.movie.Title)
       .Select(g => new MovieHoverData
       {
           MovieTitle = g.Key,
           TotalHoverTime = g.Sum(x => x.analytics.HoverDurationMs)
       })
       .OrderByDescending(x => x.TotalHoverTime)
       .Take(10)
       .ToListAsync();

        var popularGenres = await _context.MovieAnalytics
            .Where(a => cinemaId == null || a.CinemaId == cinemaId)
            .Join(
                _context.Movies.Include(m => m.Genre),
                analytics => analytics.MovieId,
                movie => movie.MovieId,
                (analytics, movie) => new { analytics, movie.Genre.Name }
            )
            .GroupBy(x => x.Name)
            .Select(g => new GenreHoverData
            {
                Genre = g.Key,
                TotalHoverTime = g.Sum(x => x.analytics.HoverDurationMs)
            })
            .OrderByDescending(x => x.TotalHoverTime)
            .ToListAsync();


        var viewModel = new CinemaAnalyticsViewModel
        {
            PopularMovies = popularMovies,
            PopularGenres = popularGenres
        };

        return View(viewModel);
    }

}
