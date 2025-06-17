using System;
using System.Collections.Generic;
using API.Data.Models;

namespace API.Data.Models;

public partial class Seat
{
    public int Id { get; set; }

    public int ScreenId { get; set; }

    public string SeatRow { get; set; } = null!;

    public int SeatNumber { get; set; }

    public int SeatTypeId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual Screen Screen { get; set; } = null!;

    public virtual SeatType SeatType { get; set; } = null!;
}
