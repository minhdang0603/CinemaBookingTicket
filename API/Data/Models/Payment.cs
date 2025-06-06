using System;
using System.Collections.Generic;
using API.Data.Models;

namespace API.Data.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int? BookingId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public DateTime PaymentDate { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public string? SessionId { get; set; }

    public string? PaymentIntentId { get; set; }

    public string? OrderCode { get; set; }

    public string? PaymentGateway { get; set; }

    public decimal? RefundAmount { get; set; }

    public DateTime? RefundDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public virtual Booking? Booking { get; set; }
}
