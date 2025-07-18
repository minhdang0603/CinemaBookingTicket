namespace API.DTOs.Response
{
    public class VNPayRefundResponseDTO
    {
        public bool Success { get; set; }
        public string VnPayResponseCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public decimal RefundAmount { get; set; }
        public DateTime RefundDate { get; set; }
        public string TransactionId { get; set; } = string.Empty;
    }
}
