namespace API.DTOs.Request
{
    public class TheaterCreateDTO
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;

        public TimeOnly? OpeningTime { get; set; }
        public TimeOnly? ClosingTime { get; set; }

        public string? Description { get; set; }
        public int ProvinceId { get; set; }
    }
}
