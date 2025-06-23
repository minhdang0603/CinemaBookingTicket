namespace Web.Models.DTOs.Request
{
    public class ShowTimeUpdateDTO
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int ScreenId { get; set; }
        public DateOnly ShowDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public decimal BasePrice { get; set; }
    }
}
