using System;
using System.Collections.Generic;

namespace Techno_Home.Models;

public partial class ProductsInOrder
{
    public int? OrderId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Stocktake? Product { get; set; }
}
