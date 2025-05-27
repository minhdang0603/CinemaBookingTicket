using System;
using System.Collections.Generic;

namespace CinemaBookingTicket_API.Data.Models;

public partial class Booking
{
    public int Id { get; set; }

    public string BookingCode { get; set; } = null!;

    public string? UserId { get; set; }

    public int ShowTimeId { get; set; }

    public DateTime BookingDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string BookingStatus { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual ICollection<ConcessionOrder> ConcessionOrders { get; set; } = new List<ConcessionOrder>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ShowTime ShowTime { get; set; } = null!;
}
