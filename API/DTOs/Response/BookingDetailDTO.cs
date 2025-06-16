namespace API.DTOs.Response;

public class BookingDetailDTO
{
    public int Id { get; set; }
    public string SeatName { get; set; } = null!;
    public decimal SeatPrice { get; set; }
}