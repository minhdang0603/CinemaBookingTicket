namespace API.DTOs.Request
{
    public class TheaterUpdateDTO
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime? OpeningTime { get; set; }  // ✅ Sửa lại từ TimeOnly? → DateTime?
        public DateTime? ClosingTime { get; set; }  // ✅
        public string? Description { get; set; }
        public int ProvinceId { get; set; }
        public bool IsActive { get; set; }
    }
}
