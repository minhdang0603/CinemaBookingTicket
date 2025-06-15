namespace API.DTOs.Response
{
    public class TheaterDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public TimeOnly? OpeningTime { get; set; }
        public TimeOnly? ClosingTime { get; set; }
        public string? Description { get; set; }
        public int ProvinceId { get; set; }
        public bool IsActive { get; set; }
    }
}
