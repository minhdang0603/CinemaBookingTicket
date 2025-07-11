using Web.Models.DTOs.Response;

namespace Web.Models.ViewModels
{
    public class PaymentViewModel
    {
        // Booking Information
        public int BookingId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string TheaterName { get; set; } = string.Empty;
        public string ShowTime { get; set; } = string.Empty;
        public string ScreenName { get; set; } = string.Empty;
        public List<string> SeatNames { get; set; } = new List<string>();
        public int SeatCount { get; set; }
        public decimal SeatTotal { get; set; }

        // Concession Information
        public int? ConcessionOrderId { get; set; }
        public List<ConcessionOrderDetailDTO> ConcessionItems { get; set; } = new List<ConcessionOrderDetailDTO>();
        public int ConcessionCount { get; set; }
        public decimal ConcessionTotal { get; set; }

        // Total
        public decimal TotalAmount { get; set; }

        // Booking Expiry
        public DateTime BookingExpiry { get; set; }
    }
}
