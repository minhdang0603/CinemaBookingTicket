using API.DTOs.Response;

namespace API.DTOs.Response;

public class BookingDTO
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public string BookingCode { get; set; }
    public DateTime BookingDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string BookingStatus { get; set; }
    public List<BookingDetailDTO> BookingItems { get; set; } = new List<BookingDetailDTO>();
    // public ShowTimeDTO ShowTime;
}
