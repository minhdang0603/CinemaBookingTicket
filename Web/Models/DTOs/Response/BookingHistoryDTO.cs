namespace Web.Models.DTOs.Response
{
    public class BookingHistoryDTO
    {
        public int BookingId { get; set; }
        public string BookingCode { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookingStatus { get; set; }
        public decimal TotalAmount { get; set; }

        // Movie info
        public string MovieTitle { get; set; }
        public string MoviePoster { get; set; }
        public string AgeRating { get; set; }

        // Theater & Show info
        public string TheaterName { get; set; }
        public string ScreenName { get; set; }
        public DateTime ShowDate { get; set; }
        public TimeSpan StartTime { get; set; }

        // Ticket count
        public int TicketCount { get; set; }

        // Payment status
        public string PaymentStatus { get; set; }
    }
}
