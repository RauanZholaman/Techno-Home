using System.ComponentModel.DataAnnotations;

namespace Techno_Home.Models;


public class ProductEditViewModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? BrandName { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public int? SubCategoryId { get; set; }
    public DateOnly? Released { get; set; }
    public decimal? Price { get; set; }

    public byte[]? Image { get; set; }
}

