namespace API.DTOs
{
    public class ProvinceUpdateDTO
    {
        public string Name { get; set; } = null!;
        public string? Code { get; set; }
        public bool IsActive { get; set; }
    }
}
