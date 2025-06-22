namespace Web.Models.DTOs.Response
{
    public class SeatTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Color { get; set; }
        public decimal PriceMultiplier { get; set; }
    }
}
