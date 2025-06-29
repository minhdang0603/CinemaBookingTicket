using System.ComponentModel.DataAnnotations;

namespace Web.Models.DTOs.Request;

public class GenreUpdateDTO
{
    public int Id { get; set; }
    [Required]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
    [MinLength(2, ErrorMessage = "Name must be at least 2 characters long.")]
    public string? Name { get; set; }
    [Required]
    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
    public string? Description { get; set; }
}
