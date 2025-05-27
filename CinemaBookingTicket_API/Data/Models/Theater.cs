using System;
using System.Collections.Generic;

namespace CinemaBookingTicket_API.Data.Models;

public partial class Theater
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public TimeOnly? OpeningTime { get; set; }

    public TimeOnly? ClosingTime { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int ProvinceId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public virtual Province Province { get; set; } = null!;

    public virtual ICollection<Screen> Screens { get; set; } = new List<Screen>();
}
