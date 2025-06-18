using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [StringLength(100)]
        public string Genre { get; set; } = string.Empty;

        public int Duration { get; set; } 

        [Range(0, 10)]
        public double Rating { get; set; }

        [Url]
        public string? PosterUrl { get; set; }

        public DateTime ReleaseDate { get; set; }

        public bool IsNowShowing { get; set; }

        public bool IsFeatured { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}