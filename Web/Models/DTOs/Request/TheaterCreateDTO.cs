using System.ComponentModel.DataAnnotations;

namespace Web.Models.DTOs.Request
{
    public class TheaterCreateDTO
    {
        [Required(ErrorMessage = "Theater name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
        public string Name { get; set; } = null!;
        
        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 200 characters")]
        public string Address { get; set; } = null!;

        [DataType(DataType.Time)]
        [Display(Name = "Opening Time")]
        public TimeOnly? OpeningTime { get; set; }
        
        [DataType(DataType.Time)]
        [Display(Name = "Closing Time")]
        public TimeOnly? ClosingTime { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Province is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid province")]
        [Display(Name = "Province")]
        public int ProvinceId { get; set; }
    }
}
