using API.DTOs.Response;

namespace API.DTOs.Request
{
    public class ShowTimeCreateDTO
    {
        public int MovieId { get; set; }
        public int ScreenId { get; set; }
        public DateOnly ShowDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public decimal BasePrice { get; set; }

    }
}
