using System;
using System.Collections.Generic;
using API.Data.Models;

namespace API.Data.Models;

public partial class ConcessionCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Concession> Concessions { get; set; } = new List<Concession>();
}
