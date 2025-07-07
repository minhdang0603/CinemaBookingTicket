using Web.Models.DTOs.Response;

namespace Web.Models.ViewModels
{
	public class ConcessionViewModel
	{
		public int ShowTimeId { get; set; }
		public string MovieTitle { get; set; }
		public string CinemaName { get; set; }
		public string ShowTime { get; set; }
		public string ScreenName { get; set; }
		public List<string> SeatName { get; set; }
		public int TotalAmount { get; set; } = 0;
		public List<ConcessionDTO> Concessions { get; set; } = new List<ConcessionDTO>();
	}
}
