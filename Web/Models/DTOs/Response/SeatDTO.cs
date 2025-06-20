namespace Web.Models.DTOs.Response;

public class SeatDTO
{
    public int Id { get; set; }
    public string SeatRow { get; set; } = null!;
    public int SeatNumber { get; set; }
    public string SeatTypeName { get; set; } = null!;
}