using System.ComponentModel.DataAnnotations;
using Web.Models.DTOs.Response;

namespace Web.Models.DTOs.Request
{
    public class ShowTimeCreateDTO : IValidatableObject
    {
        [Required(ErrorMessage = "Please select a movie.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid movie.")]
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Please select a screen.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid screen.")]
        public int ScreenId { get; set; }

        [Required(ErrorMessage = "Please select a show date.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid show date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly ShowDate { get; set; }

        [Required(ErrorMessage = "Please select a start time.")]
        [DataType(DataType.Time, ErrorMessage = "Invalid start time.")]
        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        [Required(ErrorMessage = "Please enter the base price.")]
        [Range(1000, 10000000, ErrorMessage = "Base price must be between 1,000 and 10,000,000 VND.")]
        public decimal BasePrice { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ShowDate < DateOnly.FromDateTime(DateTime.Now.Date))
            {
                yield return new ValidationResult("Show date cannot be in the past.", new[] { nameof(ShowDate) });
            }
            if (BasePrice < 1000 || BasePrice > 10000000)
            {
                yield return new ValidationResult("Base price must be between 1,000 and 10,000,000 VND.", new[] { nameof(BasePrice) });
            }
        }
    }
}
