namespace Web.Models.DTOs.Response
{
    /// <summary>
    /// A lightweight version of ShowTimeDTO without Movie information to prevent circular references
    /// </summary>
    public class ShowTimeLiteDTO
    {
        public int Id { get; set; }
        public DateOnly ShowDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public decimal BasePrice { get; set; }
        public ScreenDTO Screen { get; set; } = new ScreenDTO();
    }
}
