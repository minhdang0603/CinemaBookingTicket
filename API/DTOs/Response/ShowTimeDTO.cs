namespace API.DTOs.Response
{
    public class ShowTimeDTO
    {
        public int Id { get; set; }
        public DateOnly ShowDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public decimal BasePrice { get; set; }
        public MovieLiteDTO Movie { get; set; } = new MovieLiteDTO();
        public ScreenDTO Screen { get; set; } = new ScreenDTO();
    }
}
