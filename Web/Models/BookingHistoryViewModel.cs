using System;

namespace Web.Models
{
    public class BookingHistoryViewModel
    {
        public int Id { get; set; }
        public string BookingCode { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
        public string MovieTitle { get; set; }
        public string MoviePosterUrl { get; set; }
        public string TheaterName { get; set; }
        public string ScreenName { get; set; }
        public DateTime ShowtimeDate { get; set; }
        public TimeOnly ShowtimeTime { get; set; }
        public string SeatNames { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalTickets { get; set; }
    }
}
