namespace Web.Models.DTOs.Response;

public class ScreenDetailDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Rows { get; set; }
    public int SeatsPerRow { get; set; }
    public TheaterDTO Theater { get; set; } = null!;
    public List<SeatDTO> Seats { get; set; } = new List<SeatDTO>();
}