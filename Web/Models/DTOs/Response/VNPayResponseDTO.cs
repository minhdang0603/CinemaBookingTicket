namespace Web.Models.DTOs.Response
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
    }
}