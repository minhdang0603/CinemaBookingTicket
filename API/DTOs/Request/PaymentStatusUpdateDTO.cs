namespace API.DTOs.Request
{
    public class PaymentStatusUpdateDTO
    {
        public string Status { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
    }
}
