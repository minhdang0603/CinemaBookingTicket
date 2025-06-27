using System.ComponentModel.DataAnnotations;
using Utility;

namespace Web.Models.DTOs.Request;

public class MovieCreateDTO : IValidatableObject
{
    [Required(ErrorMessage = "Movie title is required")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 255 characters")]
    [Display(Name = "Title")]
    public string? Title { get; set; }

    [StringLength(100, ErrorMessage = "Director name cannot exceed 100 characters")]
    [Display(Name = "Director")]
    public string? Director { get; set; }

    [StringLength(500, ErrorMessage = "Cast list cannot exceed 500 characters")]
    [Display(Name = "Cast")]
    public string? Cast { get; set; }

    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Duration is required")]
    [Range(1, 500, ErrorMessage = "Duration must be between 1 and 500 minutes")]
    [Display(Name = "Duration (minutes)")]
    public int? Duration { get; set; }

    [Required(ErrorMessage = "Release date is required")]
    [Display(Name = "Release Date")]
    public DateOnly? ReleaseDate { get; set; }

    [Required(ErrorMessage = "Age rating is required")]
    [StringLength(10, ErrorMessage = "Age rating cannot exceed 10 characters")]
    [Display(Name = "Age Rating")]
    public string? AgeRating { get; set; }

    [Url(ErrorMessage = "Please enter a valid URL for the poster")]
    [StringLength(2048, ErrorMessage = "URL cannot exceed 2048 characters")]
    [Display(Name = "Poster URL")]
    public string? PosterUrl { get; set; }

    [Url(ErrorMessage = "Please enter a valid URL for the trailer")]
    [StringLength(2048, ErrorMessage = "URL cannot exceed 2048 characters")]
    [Display(Name = "Trailer URL")]
    public string? TrailerUrl { get; set; }

    [Url(ErrorMessage = "Please enter a valid URL for the background")]
    [StringLength(2048, ErrorMessage = "URL cannot exceed 2048 characters")]
    [Display(Name = "Background URL")]
    public string? BackgroundUrl { get; set; }

    [Display(Name = "Genres")]
    public List<int>? GenreIds { get; set; } = new List<int>();
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ReleaseDate.HasValue && ReleaseDate.Value > DateOnly.FromDateTime(DateTime.Now.AddYears(2)))
        {
            yield return new ValidationResult("Release date cannot be more than 2 years in the future", new[] { nameof(ReleaseDate) });
        }
        
        if (GenreIds == null || GenreIds.Count == 0)
        {
            yield return new ValidationResult("At least one genre must be selected", new[] { nameof(GenreIds) });
        }
    }
}
