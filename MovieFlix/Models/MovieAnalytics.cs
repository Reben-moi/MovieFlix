using System;


namespace MovieFlix.Models
{
    public class MovieAnalytics
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string UserId { get; set; }
        public int HoverDurationMs { get; set; }
        public DateTime HoverDate { get; set; } = DateTime.UtcNow;
        public int CinemaId { get; set; }

        public Movie Movie { get; set; }
    }
}
