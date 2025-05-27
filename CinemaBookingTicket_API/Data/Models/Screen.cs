using System;
using System.Collections.Generic;

namespace CinemaBookingTicket_API.Data.Models;

public partial class Screen
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int TheaterId { get; set; }

    public int Rows { get; set; }

    public int SeatsPerRow { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();

    public virtual Theater Theater { get; set; } = null!;
}
