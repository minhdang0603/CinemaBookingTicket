namespace API.DTOs.Request;

public class UserCreateDTO
{
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}
