namespace Web.Models.DTOs.Response;

public class ProvinceDetailDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
    public List<TheaterDTO> Theaters { get; set; }
}