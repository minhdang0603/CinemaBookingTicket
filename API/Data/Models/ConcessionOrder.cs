using System;
using System.Collections.Generic;
using API.Data.Models;

namespace API.Data.Models;

public partial class ConcessionOrder
{
    public int Id { get; set; }

    public int? BookingId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string OrderStatus { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ICollection<ConcessionOrderDetail> ConcessionOrderDetails { get; set; } = new List<ConcessionOrderDetail>();
}
