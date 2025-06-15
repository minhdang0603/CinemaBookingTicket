namespace API.DTOs.Request
{
    public class ScreenUpdateDTO
    {
        public string Name { get; set; } = null!;
        public int Rows { get; set; }
        public int SeatsPerRow { get; set; }
        public bool IsActive { get; set; }
    }
}
