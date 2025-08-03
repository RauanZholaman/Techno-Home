using Techno_Home.Models;

namespace Techno_Home.Models;

public class ProductFilter
{
    public List<Product> Products { get; set; }
    public List<Category> Categories { get; set; }
    
    public List<string>? SelectedTypes { get; set; } // For Category
    public decimal? minPrice { get; set; }
    public decimal? maxPrice { get; set; }
    public List<string> SelectedBrands { get; set; }
    
    public List<string> Alltypes { get; set; }
    public List<string> AllBrands { get; set; }
}