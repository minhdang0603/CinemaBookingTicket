namespace API.DTOs.Request;

public class ConcessionUpdateDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public int CategoryId { get; set; } // Assuming this is the ID of the category to which this concession belongs
}