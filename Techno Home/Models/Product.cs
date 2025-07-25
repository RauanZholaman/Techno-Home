using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Techno_Home.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? BrandName { get; set; }

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public int? SubCategoryId { get; set; }

    public DateOnly? Released { get; set; }
    
    public string? LastUpdatedBy { get; set; }

    public DateTime? LastUpdated { get; set; }

    public string? ImagePath { get; set; }

    public decimal? Price { get; set; }

    public virtual Category Category { get; set; } = null!;
    [ForeignKey(nameof(LastUpdatedBy))]
    public virtual User? LastUpdatedByNavigation { get; set; }

    public virtual ICollection<Stocktake> Stocktakes { get; set; } = new List<Stocktake>();

    public virtual SubCategory? SubCategory { get; set; }
}
