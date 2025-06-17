namespace API.DTOs.Request
{
    public class ScreenCreateDTO
    {
        public string Name { get; set; } = null!;
        public int TheaterId { get; set; }
        public int Rows { get; set; }
        public int SeatsPerRow { get; set; }
    }
}
