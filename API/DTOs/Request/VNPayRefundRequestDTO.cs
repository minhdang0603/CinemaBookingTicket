namespace API.DTOs.Request
{
    public class VNPayRefundRequestDTO
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string OrderInfo { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string CreateBy { get; set; } = string.Empty;
    }
}
