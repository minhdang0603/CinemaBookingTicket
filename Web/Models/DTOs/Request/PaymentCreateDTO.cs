using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.DTOs.Request
{
    public class PaymentCreateDTO
    {
        [Required]
        public int BookingId { get; set; }

        public int? ConcessionOrderId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = "vnpay";

        [Required]
        public string ReturnUrl { get; set; } = string.Empty;
    }
}
