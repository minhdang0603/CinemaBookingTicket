using API.DTOs.Request;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Request
{
    public class BookingUpdateDTO
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        public int ShowTimeId { get; set; }

    [Required]
    public List<BookingDetailItemDTO> BookingDetails { get; set; } = new();
    }
}
