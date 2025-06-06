using System;
using System.Collections.Generic;
using API.Data.Models;

namespace API.Data.Models;

public partial class Province
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Code { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Theater> Theaters { get; set; } = new List<Theater>();
}
