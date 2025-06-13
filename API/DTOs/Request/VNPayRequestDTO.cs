namespace API.DTOs.Request;

public class VNPayRequestDTO
{
    public int BookingId { get; set; }
    public double Amount { get; set; }
    public string OrderInfo { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
    public string ClientIpAddress { get; set; } = string.Empty;
}