namespace API.DTOs.Response;

public class ConcessionDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ConcessionCategoryDTO Category { get; set; } = new ConcessionCategoryDTO();
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
}