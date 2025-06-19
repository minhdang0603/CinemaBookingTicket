namespace Web.Models.DTOs.Request
{
    public class ScreenCreateDTO
    {
        public string? Name { get; set; }
        public int? TheaterId { get; set; }
        public int? Rows { get; set; }
        public int? SeatsPerRow { get; set; }
    }
}
