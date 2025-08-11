
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieFlix.Models;

namespace MovieFlix.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<Movie> Movies { get; set; } = new();
        public List<SelectListItem> Cinemas { get; set; }
        public int? SelectedGenreId { get; set; }
        public int? SelectedCinemaId { get; set; }
        public List<SelectListItem> Genres { get; set; } = new();
        public string? SearchQuery { get; set; }
    
    }

}