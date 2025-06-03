using System;
using System.Collections.Generic;
using API.Data.Models;

namespace API.Data.Models;

public partial class SeatType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal PriceMultiplier { get; set; }

    public string? Color { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
