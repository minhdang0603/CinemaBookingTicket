using API.DTOs.Request;

namespace API.DTOs.Response
{
    public class ShowTimeBulkResultDTO
    {
        public List<ShowTimeDTO> SuccessfulShowTimes { get; set; } = new List<ShowTimeDTO>();
        public List<string> FailedShowTimes { get; set; } = new List<string>();
    }
}
