using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace Techno_Home.Controllers;

public class HelloWorldController : Controller
{
    private int res = 0;

    private int x = 2;
    private int y = 4;
    // 
    // GET: /HelloWorld/
    public IActionResult Index()
    {
        return View();
    }
    // 
    // GET: /HelloWorld/Welcome/ 
    public IActionResult Welcome(string name, int numTimes = 1)
    {
        ViewData["Message"] = "Hello, " + name;
        ViewData["NumTimes"] = numTimes;
        return View();
    }
    
    public string Hui()
    {
        res = x * y;
        return "Gay Sex = " + res ;
    }
}