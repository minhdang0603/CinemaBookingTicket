using System;
using System.Collections.Generic;

namespace CinemaBookingTicket_API.Data.Models;

public partial class BookingDetail
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public int SeatId { get; set; }

    public string SeatName { get; set; } = null!;

    public decimal SeatPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Seat Seat { get; set; } = null!;
}
