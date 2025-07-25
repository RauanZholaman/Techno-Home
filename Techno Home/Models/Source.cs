using System;
using System.Collections.Generic;

namespace Techno_Home.Models;

public partial class Source
{
    public int Sourceid { get; set; }

    public string? SourceName { get; set; }

    public string? ExternalLink { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Stocktake> Stocktakes { get; set; } = new List<Stocktake>();
}
