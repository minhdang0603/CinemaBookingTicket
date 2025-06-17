namespace API.DTOs.Response
{
    public class ScreenDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }

    }
}