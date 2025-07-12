using System.Collections.Generic;

namespace Web.Models.DTOs.Request
{
    public class BookingUpdateDTO
    {
        public int BookingId { get; set; }
        public int ShowTimeId { get; set; }
        public List<BookingDetailItemDTO> BookingDetails { get; set; } = new List<BookingDetailItemDTO>();
    }
}
