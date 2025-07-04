using Web.Models.DTOs.Response;

namespace Web.Models.DTOs.Response
{
    public class HomeMoviesDTO
    {
        public List<MovieDTO> NowShowing { get; set; } = new List<MovieDTO>();
        public List<MovieDTO> ComingSoon { get; set; } = new List<MovieDTO>();
    }
}
