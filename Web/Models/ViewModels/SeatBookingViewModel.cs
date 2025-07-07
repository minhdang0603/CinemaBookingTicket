using Web.Models.DTOs.Response;

namespace Web.Models.ViewModels
{
    public class SeatBookingViewModel
    {
        public int ShowtimeId { get; set; }
        public int MovieId { get; set; }
		public string ScreenName { get; set; } = string.Empty;
        public string MovieTitle { get; set; } = string.Empty;
        public string TheaterName { get; set; } = string.Empty;
        public DateTime ShowtimeDate { get; set; }
        public TimeOnly ShowtimeTime { get; set; }
        public List<SeatBookingStatusDTO> Seats { get; set; } = new List<SeatBookingStatusDTO>();
        public List<SeatTypeViewModel> SeatTypes { get; set; } = new List<SeatTypeViewModel>();
    }

    public class SeatTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Color { get; set; } = string.Empty;
		public string PriceFormatted => Price.ToString("N0") + "₫";
    }
}
