using System;
using System.Collections.Generic;

namespace CinemaBookingTicket_API.Data.Models;

public partial class Concession
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsAvailable { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public virtual ConcessionCategory Category { get; set; } = null!;

    public virtual ICollection<ConcessionOrderDetail> ConcessionOrderDetails { get; set; } = new List<ConcessionOrderDetail>();
}
