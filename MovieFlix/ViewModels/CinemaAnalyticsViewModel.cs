namespace MovieFlix.ViewModels
{
    public class CinemaAnalyticsViewModel
    {
        public List<MovieHoverData> PopularMovies { get; set; }
        public List<GenreHoverData> PopularGenres { get; set; }
    }

    public class MovieHoverData
    {
        public string MovieTitle { get; set; }
        public int TotalHoverTime { get; set; }
    }

    public class GenreHoverData
    {
        public string Genre { get; set; }
        public int TotalHoverTime { get; set; }
    }

}
