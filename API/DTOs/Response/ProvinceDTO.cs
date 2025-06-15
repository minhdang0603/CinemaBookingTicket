namespace API.DTOs.Response
{
    public class ProvinceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Code { get; set; }
        public bool IsActive { get; set; }
    }
}
