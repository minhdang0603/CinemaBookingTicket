using System.ComponentModel.DataAnnotations;

namespace Web.Models.DTOs.Request
{
    public class ProvinceCreateDTO
    {
        [Required(ErrorMessage = "Province name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        [Display(Name = "Province Name")]
        public string? Name { get; set; }
        
        [Required(ErrorMessage = "Province code is required")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Code must be between 1 and 10 characters")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Code must contain only uppercase letters and numbers")]
        [Display(Name = "Province Code")]
        public string? Code { get; set; }
    }
}
