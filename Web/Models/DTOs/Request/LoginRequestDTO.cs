using System.ComponentModel.DataAnnotations;

namespace Web.Models.DTOs.Request;

public class LoginRequestDTO
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [Display(Name = "Email Address")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string? Password { get; set; }
}
