namespace API.DTOs.Response;

public class SeatDTO
{
    public int Id { get; set; }
    public string SeatRow { get; set; } = null!;
    public int SeatNumber { get; set; }
    public SeatTypeDTO SeatType { get; set; }
}