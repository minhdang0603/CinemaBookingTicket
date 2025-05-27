using System;
using System.Collections.Generic;

namespace CinemaBookingTicket_API.Data.Models;

public partial class MovieGenre
{
    public int Id { get; set; }

    public int MovieId { get; set; }

    public int GenreId { get; set; }

    public DateTime CreatedAt { get; set; }
}
