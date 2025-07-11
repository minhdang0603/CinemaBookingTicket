using Web.Models.DTOs.Response;

namespace Web.Models.ViewModels
{
	public class ConcessionViewModel
	{
		public int ShowTimeId { get; set; }
		public int BookingId { get; set; }
		public string MovieTitle { get; set; } = string.Empty;
		public string CinemaName { get; set; } = string.Empty;
		public string ShowTime { get; set; } = string.Empty;
		public string ScreenName { get; set; } = string.Empty;
		public List<string> SeatName { get; set; } = new List<string>();
		public int TotalAmount { get; set; } = 0;
		public List<ConcessionDTO> Concessions { get; set; } = new List<ConcessionDTO>();
	}
}
