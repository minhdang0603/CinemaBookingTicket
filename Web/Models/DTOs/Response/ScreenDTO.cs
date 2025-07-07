using System.Collections.Generic;

namespace Web.Models.DTOs.Response
{
    public class ScreenDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int Rows { get; set; }

        public int SeatsPerRow { get; set; }

        public TheaterDTO? Theater { get; set; }
    }
}
