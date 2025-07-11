namespace API.DTOs.Response
{
    public class VNPayResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string ResponseCode { get; set; } = string.Empty;
        public string TransactionStatus { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string? RedirectUrl { get; set; }

        // Thông tin booking được thêm vào response
        public string BookingCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string MovieTitle { get; set; } = string.Empty;
        public string TheaterName { get; set; } = string.Empty;
        public string ScreenName { get; set; } = string.Empty;
        public DateTime ShowDate { get; set; }
        public TimeOnly ShowTime { get; set; }
        public List<string> SeatNames { get; set; } = new List<string>();

        // Thêm full booking object (optional, sẽ được populate nếu payment success)
        public BookingDTO? Booking { get; set; }
    }
}