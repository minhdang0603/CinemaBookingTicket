using System.ComponentModel.DataAnnotations;

namespace Web.Models.ViewModels
{
    public class MovieCreateViewModel
    {
        [Required(ErrorMessage = "Movie title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Genre is required")]
        [StringLength(100, ErrorMessage = "Genre cannot exceed 100 characters")]
        public string Genre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 500, ErrorMessage = "Duration must be between 1 and 500 minutes")]
        public int Duration { get; set; }

        [Range(0, 10, ErrorMessage = "Rating must be between 0 and 10")]
        public double Rating { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? PosterUrl { get; set; }

        [Required(ErrorMessage = "Release date is required")]
        public DateTime ReleaseDate { get; set; } = DateTime.Now;

        public bool IsNowShowing { get; set; }

        public bool IsFeatured { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, 1000000, ErrorMessage = "Price must be a positive number")]
        public decimal Price { get; set; }
    }

    public class MovieEditViewModel : MovieCreateViewModel
    {
        public int Id { get; set; }
    }
}