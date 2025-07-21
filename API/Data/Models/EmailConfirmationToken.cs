using System.ComponentModel.DataAnnotations;

namespace API.Data.Models
{
    public class EmailConfirmationToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; } = false;

        // Navigation property
        public virtual ApplicationUser User { get; set; }
    }
}
