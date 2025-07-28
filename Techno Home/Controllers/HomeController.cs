using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Techno_Home.Data;
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

    // public async Task<IActionResult> Catalog()
    // {
    //     var products = await _context.Products
    //         .Select(p => new Product
    //         {
    //             Id = p.Id,
    //             Name = p.Name,
    //             BrandName = p.BrandName,
    //             Description = p.Description,
    //             CategoryId = p.CategoryId,
    //             SubCategoryId = p.SubCategoryId,
    //             Released = p.Released,
    //             LastUpdatedBy = p.LastUpdatedBy,
    //             LastUpdated = p.LastUpdated,
    //             ImagePath = p.ImagePath,
    //             Price = p.Price
    //             // Deliberately NOT including LastUpdatedByNavigation
    //         })
    //         .AsNoTracking()
    //         .ToListAsync();
    //
    //     return View(products);
    // }
    
    public async Task<IActionResult> Catalog()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .ToListAsync();

        var categories = await _context.Categories.ToListAsync();

        var viewModel = new ProductFilter
        {
            Products = products,
            Categories = categories,
            Alltypes = categories.Select(c => c.Name).ToList(),
            AllBrands = products.Select(p => p.BrandName).Distinct().ToList()
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