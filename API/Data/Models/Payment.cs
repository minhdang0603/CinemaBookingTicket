using System;
using System.Collections.Generic;
using API.Data.Models;

namespace API.Data.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int? BookingId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public string? OrderCode { get; set; }

    public decimal? RefundAmount { get; set; }

    public DateTime? RefundDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public string? TransactionId { get; set; }

    public string? BankCode { get; set; }

    public string? BankTransactionNo { get; set; }

    public string? CardType { get; set; }

    public string? ResponseCode { get; set; }

    public string? SecureHash { get; set; }

    public virtual Booking? Booking { get; set; }
}
