namespace Web.Models.DTOs.Response
{
    public class TheaterDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public TimeOnly? OpeningTime { get; set; }
        public TimeOnly? ClosingTime { get; set; }

        public string? Description { get; set; }
        public ProvinceDTO? Province { get; set; }
    }
}
