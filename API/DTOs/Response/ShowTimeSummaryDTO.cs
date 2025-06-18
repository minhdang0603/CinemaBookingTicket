namespace API.DTOs.Response
{
    /// <summary>
    /// A simplified version of ShowTime information for use in BookingDTO
    /// </summary>
    public class ShowTimeSummaryDTO
    {
        public int Id { get; set; }
        public DateOnly ShowDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string TheaterName { get; set; } = string.Empty;
        public string ScreenName { get; set; } = string.Empty;
    }
}
