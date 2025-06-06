namespace API.DTOs.Response;

public class LoginResponseDTO
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
    public UserDTO User { get; set; }
}
