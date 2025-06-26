namespace Web.Models.DTOs.Response;

public class ConcessionOrderDetailDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal UnitPrice { get; set; }
    public string ImageUrl { get; set; }
    public int Quantity { get; set; }
}