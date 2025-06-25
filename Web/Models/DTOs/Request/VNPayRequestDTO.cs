namespace Web.Models.DTOs.Request;

public class VNPayRequestDTO
{
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
    public string OrderInfo { get; set; } = string.Empty;
    public string ClientIpAddress { get; set; } = string.Empty;
}