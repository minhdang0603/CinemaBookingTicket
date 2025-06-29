

namespace Web.Models.DTOs.Response;

public class ConcessionDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public ConcessionCategoryDTO? Category { get; set; } = null;
    public List<ConcessionOrderDetailDTO>? ConcessionOrderDetails { get; set; } = new List<ConcessionOrderDetailDTO>();
}