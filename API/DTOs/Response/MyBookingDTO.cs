namespace API.DTOs.Response;

public class MyBookingDTO
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string BookingCode { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string BookingStatus { get; set; } = string.Empty;
    public List<BookingDetailDTO> BookingItems { get; set; } = new List<BookingDetailDTO>();
    public MyShowTimeDTO ShowTime { get; set; } = new MyShowTimeDTO();
}