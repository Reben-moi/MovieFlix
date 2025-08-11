using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieFlix.Models;

namespace MovieFlix.Data {
    public class MovieflixContext : IdentityDbContext<ApplicationUser>
    {

        public MovieflixContext(DbContextOptions<MovieflixContext> options)
            : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }

        public DbSet<Genre> Genres { get; set; }
    }
  
}