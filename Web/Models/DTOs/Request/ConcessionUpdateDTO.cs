using System.ComponentModel.DataAnnotations;

namespace Web.Models.DTOs.Request;

public class ConcessionUpdateDTO
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    [Display(Name = "Name")]
    public string Name { get; set; } = null!;
    
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    [Display(Name = "Description")]
    public string Description { get; set; } = null!;
    
    [Required(ErrorMessage = "Price is required")]
    [Range(1000, 1000000, ErrorMessage = "Price must be between 1,000 and 1,000,000")]
    [DataType(DataType.Currency)]
    [Display(Name = "Price")]
    public decimal Price { get; set; }
    
    [Url(ErrorMessage = "Please enter a valid URL for the image")]
    [StringLength(2048, ErrorMessage = "URL cannot exceed 2048 characters")]
    [Display(Name = "Image URL")]
    public string ImageUrl { get; set; } = null!;
    
    [Required(ErrorMessage = "Category is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }
}