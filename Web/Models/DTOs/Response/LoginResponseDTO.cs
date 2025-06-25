using Web.Models.DTOs.Response;

namespace Web.Models.DTOs.Response;

public class LoginResponseDTO
{
    public string? Token { get; set; }
    public DateTime Expiration { get; set; }
}
