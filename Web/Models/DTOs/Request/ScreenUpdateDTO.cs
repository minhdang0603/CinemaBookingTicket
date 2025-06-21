using System.Collections.Generic;

namespace Web.Models.DTOs.Request
{
    public class ScreenUpdateDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Rows { get; set; }
        public int? SeatsPerRow { get; set; }
        public List<SeatUpdateDTO>? Seats { get; set; } = new List<SeatUpdateDTO>();
    }
}
