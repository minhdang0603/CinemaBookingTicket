namespace Web.Models.DTOs.Request;

public class ConcessionCategoryUpdateDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}