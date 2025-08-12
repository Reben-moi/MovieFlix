using Microsoft.AspNetCore.Http;
using MovieFlix.Data;
using MovieFlix.Models;
using System.Security.Claims;


namespace MovieFlix.Services
{
    public class AuditService
    {
        private readonly MovieflixContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(MovieflixContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(string action, string entityName, int? entityId = null, string details = null)
        {
            var userId = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Timestamp = DateTime.UtcNow,
                Details = details
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }

}
