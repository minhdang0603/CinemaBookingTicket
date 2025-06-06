using System;
using System.Collections.Generic;
using API.Data.Models;

namespace API.Data.Models;

public partial class ShowTime
{
    public int Id { get; set; }

    public int MovieId { get; set; }

    public int ScreenId { get; set; }

    public DateOnly ShowDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public decimal BasePrice { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Movie Movie { get; set; } = null!;

    public virtual Screen Screen { get; set; } = null!;
}
