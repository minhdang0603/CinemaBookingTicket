namespace Web.Models.DTOs.Response;

public class BookingDTO
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string BookingCode { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string BookingStatus { get; set; } = string.Empty;
    public List<BookingDetailDTO> BookingItems { get; set; } = new List<BookingDetailDTO>();
    public ShowTimeSummaryDTO ShowTime { get; set; } = new ShowTimeSummaryDTO();
}
