using System;

namespace Web.Models.DTOs.Response
{
    public class SeatBookingStatusDTO
    {
        public int Id { get; set; }
        public string SeatRow { get; set; } = string.Empty;
        public int SeatNumber { get; set; }
        public string SeatCode => $"{SeatRow}{SeatNumber}";
        public SeatTypeDTO SeatType { get; set; } = new SeatTypeDTO();
        public decimal Price { get; set; }
        public bool IsBooked { get; set; }
    }

    public class ShowTimeSeatStatusDTO
    {
        public int ShowTimeId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public DateTime ShowDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public string ScreenName { get; set; } = string.Empty;
        public string TheaterName { get; set; } = string.Empty;
		public decimal BasePrice { get; set; }
        public List<SeatBookingStatusDTO> Seats { get; set; } = new List<SeatBookingStatusDTO>();
    }
}
