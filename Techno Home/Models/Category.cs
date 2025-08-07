using System;
using System.Collections.Generic;

namespace Techno_Home.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Source> Sources { get; set; } = new List<Source>();

    public virtual ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
}
