using System;
using System.Collections.Generic;

namespace CinemaBookingTicket_API.Data.Models;

public partial class ConcessionOrderDetail
{
    public int Id { get; set; }

    public int ConcessionOrderId { get; set; }

    public int ConcessionId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public virtual Concession Concession { get; set; } = null!;

    public virtual ConcessionOrder ConcessionOrder { get; set; } = null!;
}
