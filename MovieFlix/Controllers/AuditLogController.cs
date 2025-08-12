using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;  // for UserManager
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieFlix.Data;
using MovieFlix.Models;  
using MovieFlix.ViewModels;  
using System.Linq;
using System.Threading.Tasks;



[Authorize(Roles = "SuperAdmin")]
public class AuditLogController : Controller
{
    private readonly MovieflixContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuditLogController(MovieflixContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();  // View with empty table that DataTables will fill
    }

    [HttpGet]
    public async Task<IActionResult> GetAuditLogs()
    {
        var logs = await _context.AuditLogs
            .OrderByDescending(l => l.Timestamp)
            .Take(100)
            .ToListAsync();

        var data = new List<object>();

        foreach (var log in logs)
        {
            var user = await _userManager.FindByIdAsync(log.UserId);
            data.Add(new
            {
                timestamp = log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                username = user?.UserName ?? "Unknown",
                action = log.Action,
                entityName = log.EntityName,
                entityId = log.EntityId,
                details = log.Details
            });
        }

        return Json(new { data });
    }
}


