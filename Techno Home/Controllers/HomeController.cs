using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Techno_Home.Models;
using StoreDbContext = Techno_Home.Data.StoreDbContext;

namespace Techno_Home.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly StoreDbContext _context;
    
    public HomeController(ILogger<HomeController> logger, StoreDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    
    [HttpGet]
    public async Task<IActionResult> Catalog(List<string> type, List<string> brand, decimal? minPrice, decimal? maxPrice)
    {
        var allProducts = _context.Products.Include(p => p.Category).AsQueryable();

        if (type != null && type.Any())
            allProducts = allProducts.Where(p => type.Contains(p.Category.Name));

        if (brand != null && brand.Any())
            allProducts = allProducts.Where(p => brand.Contains(p.BrandName));

        if (minPrice.HasValue)
            allProducts = allProducts.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            allProducts = allProducts.Where(p => p.Price <= maxPrice.Value);

        var products = await allProducts.AsNoTracking().ToListAsync();
        var categories = await _context.Categories.ToListAsync();

        var viewModel = new ProductFilter
        {
            Products = products,
            Categories = categories,
            Alltypes = categories.Select(c => c.Name).ToList(),
            AllBrands = await _context.Products.Select(p => p.BrandName).Distinct().ToListAsync()
        };

        return View(viewModel);
    }

    
    
    public IActionResult About()
    {
        return View();
    }
    
    public IActionResult FAQ()
    {
        return View();
    }
    
    public IActionResult Contacts()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}