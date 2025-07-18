using Web.Models.DTOs.Response;

namespace Web.Models.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<MovieDTO> FeaturedMovies { get; set; } = new List<MovieDTO>();
        public bool HasMovies => FeaturedMovies != null && FeaturedMovies.Any();
        public string ErrorMessage { get; set; } = string.Empty;
        public bool IsLoading { get; set; } = false;

        // Constructor
        public HomeIndexViewModel()
        {
            FeaturedMovies = new List<MovieDTO>();
        }

        // Helper method để debug
        public string GetDebugInfo()
        {
            return $"Movies count: {FeaturedMovies?.Count ?? 0}, HasMovies: {HasMovies}, Error: {ErrorMessage}";
        }

    }
}
