namespace API.DTOs.Response
{
    public class TheaterDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime? OpeningTime { get; set; }
        public DateTime? ClosingTime { get; set; }

        public string? Description { get; set; }
        public ProvinceDTO? Province { get; set; }
    }
}
