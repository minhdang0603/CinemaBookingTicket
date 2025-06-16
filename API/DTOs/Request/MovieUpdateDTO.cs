using Utility;

namespace API.DTOs.Request;

public class MovieUpdateDTO
{

    public string Title { get; set; } = string.Empty;

    public string Director { get; set; } = string.Empty;

    public string Cast { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Duration { get; set; }

    public DateOnly ReleaseDate { get; set; }

    public Constant.AgeRatingType AgeRating { get; set; }

    public string PosterUrl { get; set; } = string.Empty;

    public string TrailerUrl { get; set; } = string.Empty;

    public string BackgroundUrl { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public List<int> GenreIds { get; set; } = new List<int>();
}
