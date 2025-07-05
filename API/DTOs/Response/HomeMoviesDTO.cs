using API.DTOs.Response;

namespace API.DTOs.Response
{
    public class HomeMoviesDTO
    {
        public List<MovieDTO> NowShowing { get; set; } = new List<MovieDTO>();
        public List<MovieDTO> ComingSoon { get; set; } = new List<MovieDTO>();
    }
}
