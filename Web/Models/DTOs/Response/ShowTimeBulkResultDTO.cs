using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;

namespace Web.Models.DTOs.Response
{
    public class ShowTimeBulkResultDTO
    {
        public List<ShowTimeDTO> SuccessfulShowTimes { get; set; } = new List<ShowTimeDTO>();
        public List<ShowTimeFailureDTO> FailedShowTimes { get; set; } = new List<ShowTimeFailureDTO>();
    }

    public class ShowTimeFailureDTO
    {
        public ShowTimeCreateDTO ShowTime { get; set; }
        public string ErrorMessage { get; set; }
    }
}
