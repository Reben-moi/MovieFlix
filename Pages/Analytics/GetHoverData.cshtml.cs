using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class GetHoverDataModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public GetHoverDataModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        // Aggregate hover counts by MovieId, excluding admin users
        var hoverData = await _context.UserInteractions
            .Where(ui => ui.Action == "HoverMovie")
            .GroupBy(ui => ui.MovieId)
            .Select(g => new
            {
                MovieId = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        // Get movie titles for labels
        var movieIds = hoverData.Select(h => h.MovieId).ToList();
        var movies = await _context.Movies
            .Where(m => movieIds.Contains(m.MovieId))
            .ToDictionaryAsync(m => m.MovieId, m => m.Title);

        var labels = hoverData.Select(h => movies.ContainsKey(h.MovieId) ? movies[h.MovieId] : $"Movie {h.MovieId}").ToList();
        var counts = hoverData.Select(h => h.Count).ToList();

        return new JsonResult(new { labels, counts });
    }
}

fetch('/Analytics/GetHoverData')
  .then(res => res.json())
  .then(data => {
    // Use data.labels and data.counts for your chart
    });