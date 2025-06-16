namespace API.DTOs.Response
{
    public class ScreenDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int TheaterId { get; set; }
        public int Rows { get; set; }
        public int SeatsPerRow { get; set; }
        public bool IsActive { get; set; }
    }
}
