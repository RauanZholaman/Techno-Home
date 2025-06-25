using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Techno_Home.Data;
using Techno_Home.Models;

namespace Techno_Home.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly Techno_HomeContext _context;
    
    public HomeController(ILogger<HomeController> logger, Techno_HomeContext context)
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

    public async Task<IActionResult> Catalog()
    {
        return View(await _context.Product.ToListAsync());
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