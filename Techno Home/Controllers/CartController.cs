using Microsoft.AspNetCore.Mvc;
using Techno_Home.Services;

namespace Techno_Home.Controllers;

public class CartController : Controller
{
    private readonly ShoppingCartService _cart;

    public CartController(ShoppingCartService cart)
    {
        _cart = cart;
    }
    
    // GET
    public IActionResult Index()
    {
        var items = _cart.GetCartItems();
        ViewBag.Total = _cart.GetTotal();
        return View(items);
    }

    public IActionResult Add(int id)
    {
        _cart.AddToCart(id);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult UpdateQuantity(int productId, int quantity)
    {
        _cart.UpdateQuantity(productId, quantity);
        return RedirectToAction("Index");
    }
    
    public IActionResult Remove(int id)
    {
        _cart.RemoveFromCart(id);
        return RedirectToAction("Index");
    }

    public IActionResult Clear()
    {
        _cart.ClearCart();
        return RedirectToAction("Index");
    }
}