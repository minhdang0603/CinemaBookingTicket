using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.DTOs.Request
{
    public class ScreenUpdateDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Screen name is required.")]
        [StringLength(50, ErrorMessage = "Screen name must not exceed 50 characters.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Number of rows is required.")]
        [Range(1, 26, ErrorMessage = "Number of rows must be between 1 and 26.")]
        public int? Rows { get; set; }

        [Required(ErrorMessage = "Seats per row is required.")]
        [Range(1, 20, ErrorMessage = "Seats per row must be between 1 and 20.")]
        public int? SeatsPerRow { get; set; }

        [Required(ErrorMessage = "Please select a theater.")]
        public int? TheaterId { get; set; }

        public List<SeatUpdateDTO>? Seats { get; set; } = new List<SeatUpdateDTO>();
    }
}
