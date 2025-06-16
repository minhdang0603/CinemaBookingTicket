namespace API.DTOs.Request;

public class BookingDetailsCreateDTO
{
    public int BookingId { get; set; }

    public List<BookingDetailItemDTO> BookingDetails { get; set; } = new List<BookingDetailItemDTO>();
}

public class BookingDetailItemDTO
{
    public int SeatId { get; set; }
    public string SeatName { get; set; }
    public decimal SeatPrice { get; set; }
}