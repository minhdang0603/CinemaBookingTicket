namespace API.DTOs.Response;

public class MovieDetailDTO
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Director { get; set; }

    public string? Cast { get; set; }

    public string? Description { get; set; }

    public int Duration { get; set; }

    public DateOnly ReleaseDate { get; set; }

    public string? AgeRating { get; set; }

    public string? PosterUrl { get; set; }

    public string? TrailerUrl { get; set; }

    public string? BackgroundUrl { get; set; }

    public string? Status { get; set; }

    public List<GenreDTO>? Genres { get; set; }

    public List<ShowTimeLiteDTO>? ShowTimes { get; set; }
}