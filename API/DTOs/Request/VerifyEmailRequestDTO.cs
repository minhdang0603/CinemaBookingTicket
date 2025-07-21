using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Request
{
    public class VerifyEmailRequestDTO
    {
        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; } = string.Empty;
    }
}
