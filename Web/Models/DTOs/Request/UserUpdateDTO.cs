namespace Web.Models.DTOs.Request
{
    public class UserUpdateDTO
    {
        public string? UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}