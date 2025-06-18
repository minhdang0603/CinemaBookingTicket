namespace API.DTOs.Response
{
    /// <summary>
    /// A lightweight version of MovieDTO without ShowTimes to prevent circular references
    /// </summary>
    public class MovieLiteDTO
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public int Duration { get; set; }

        public string? PosterUrl { get; set; }

        public string? AgeRating { get; set; }
    }
}
