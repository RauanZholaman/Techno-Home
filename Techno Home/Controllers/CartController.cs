using Microsoft.AspNetCore.Mvc;
using Techno_Home.Services;

namespace Techno_Home.Controllers;

public class CartController : Controller
{
    // Injected service to manage shopping cart operations
    private readonly ShoppingCartService _cart; 

    public CartController(ShoppingCartService cart)
    {
        _cart = cart;
    }
    
    // Displays the contents of the shopping cart
    // GET
    public IActionResult Index()
    {
        var items = _cart.GetCartItems();
        ViewBag.Total = _cart.GetTotal();
        return View(items);
    }

    // Adds a product to the cart based on its ID
    public IActionResult Add(int id)
    {
        _cart.AddToCart(id);
        return RedirectToAction("Index");
    }

    // Updates quantity of a specific product in the cart
    [HttpPost]
    public IActionResult UpdateQuantity(int productId, int quantity)
    {
        _cart.UpdateQuantity(productId, quantity); // Update quantity of selected item
        return RedirectToAction("Index");
    }
    
    // Removes a product from the cart based on its ID
    public IActionResult Remove(int id)
    {
        _cart.RemoveFromCart(id);
        return RedirectToAction("Index");
    }

    // Clears all items from the cart
    public IActionResult Clear()
    {
        _cart.ClearCart();
        return RedirectToAction("Index");
    }
}