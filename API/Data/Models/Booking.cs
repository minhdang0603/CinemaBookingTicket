 using System;
using System.Collections.Generic;
using API.Data.Models;

namespace API.Data.Models;

public partial class Booking
{
    public int Id { get; set; }

    public string BookingCode { get; set; } = null!;

    public int ShowTimeId { get; set; }

    public DateTime BookingDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string BookingStatus { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual ICollection<ConcessionOrder> ConcessionOrders { get; set; } = new List<ConcessionOrder>();

    public virtual Payment Payment { get; set; }

    public virtual ApplicationUser ApplicationUser { get; set; }

    public virtual ShowTime ShowTime { get; set; } = null!;
}
