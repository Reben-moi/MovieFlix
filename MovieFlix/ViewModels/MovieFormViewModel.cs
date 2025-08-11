using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MovieFlix.ViewModels
{
    public class MovieFormViewModel
    {
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10.")]
        public double Rating { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Release date is required.")]
        public DateTime? ReleaseDate { get; set; } // Change to nullable DateTime

        [Display(Name = "Genre")]
        [Required(ErrorMessage = "Genre  is required.")]
        public int GenreId { get; set; }
        public IEnumerable<SelectListItem>? Genres { get; set; }

        
        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }
       
        public IFormFile? ImageFile { get; set; }


        [Required]
        [Display(Name = "Cinema Name")]
        public int? CinemaId { get; set; } 
        public List<SelectListItem> Cinemas { get; set; } // for the dropdown



    }
}
