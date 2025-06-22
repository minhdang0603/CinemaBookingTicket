using System.Collections.Generic;
using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;

namespace Web.Models.ViewModels
{
    public class SeatConfigurationViewModel
    {
        public int ScreenId { get; set; }
        public int Rows { get; set; }
        public int SeatsPerRow { get; set; }
        public List<SeatDTO> ExistingSeats { get; set; } = new List<SeatDTO>();
    }
}
