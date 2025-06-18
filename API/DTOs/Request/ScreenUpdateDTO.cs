using System.Collections.Generic;

namespace API.DTOs.Request
{
    public class ScreenUpdateDTO
    {
        public string? Name { get; set; }
        public int? Rows { get; set; }
        public int? SeatsPerRow { get; set; }
        public List<SeatUpdateDTO>? Seats { get; set; } = new List<SeatUpdateDTO>();
    }
}
