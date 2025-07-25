﻿using System;
using System.Collections.Generic;

namespace Techno_Home.Models;

public partial class SubCategory
{
    public int SubCategoryId { get; set; }

    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
