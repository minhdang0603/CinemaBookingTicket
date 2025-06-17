namespace API.DTOs.Response;

public class ScreenDetailDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<SeatDTO> Seats { get; set; }
}