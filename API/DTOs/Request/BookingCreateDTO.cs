namespace API.DTOs.Request;

public class BookingCreateDTO
{
    public int RoomId { get; set; }
    public int UserId { get; set; }
    public int ShowTimeId { get; set; }
    public List<BookingDetailItemDTO> BookingDetails { get; set; } = new List<BookingDetailItemDTO>();
}

public class BookingDetailItemDTO
{
    public int SeatId { get; set; }
    public string SeatName { get; set; }
    public decimal SeatPrice { get; set; }
}