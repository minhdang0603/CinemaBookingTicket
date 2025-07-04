using Web.Models.DTOs.Response;

namespace Web.Models.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<MovieDTO> FeaturedMovies { get; set; } = new List<MovieDTO>();
        public bool HasMovies => FeaturedMovies?.Any() == true;
    }
}
