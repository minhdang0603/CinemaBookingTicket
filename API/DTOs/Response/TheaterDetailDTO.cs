namespace API.DTOs.Response;

public class TheaterDetailDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;

    public DateTime? OpeningTime { get; set; }
    public DateTime? ClosingTime { get; set; }

    public string? Description { get; set; }

    public List<ScreenLiteDTO> Screens { get; set; } = new List<ScreenLiteDTO>();
}