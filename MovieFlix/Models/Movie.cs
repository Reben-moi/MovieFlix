namespace MovieFlix.Models
{
    public class Movie
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Rating { get; set; }

        public DateTime? ReleaseDate { get; set; } // Make ReleaseDate nullable
        public string ImageUrl { get; set; }

        //cinema items
        public int? CinemaId { get; set; }
        public Cinema Cinema { get; set; }

        // Genre items
        public int GenreId { get; set; }
       
        public Genre Genre { get; set; }
    }

}
