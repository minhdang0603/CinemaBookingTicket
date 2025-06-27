using System.ComponentModel.DataAnnotations;
using Web.Models.DTOs.Response;

namespace Web.Models.DTOs.Request
{
    public class ShowTimeCreateDTO : IValidatableObject
    {
        [Required(ErrorMessage = "Please select a movie")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid movie selection")]
        [Display(Name = "Movie")]
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Please select a screen")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid screen selection")]
        [Display(Name = "Screen")]
        public int ScreenId { get; set; }

        [Required(ErrorMessage = "Show date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Show Date")]
        public DateOnly ShowDate { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        [DataType(DataType.Time)]
        [Display(Name = "Start Time")]
        public TimeOnly StartTime { get; set; }

        // EndTime is calculated automatically based on StartTime and movie Duration
        public TimeOnly EndTime { get; set; }

        [Required(ErrorMessage = "Base price is required")]
        [Range(1000, 10000000, ErrorMessage = "Base price must be between 1,000 and 10,000,000 VND")]
        [DataType(DataType.Currency)]
        [Display(Name = "Base Price")]
        public decimal BasePrice { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ShowDate < DateOnly.FromDateTime(DateTime.Now.Date))
            {
                yield return new ValidationResult("Show date cannot be in the past", new[] { nameof(ShowDate) });
            }
            // BasePrice validation is already handled by Range attribute
        }
    }
}
