using Microsoft.AspNetCore.Identity;

namespace MovieFlix.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? CinemaId { get; set; }
    }
}
