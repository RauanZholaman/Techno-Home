using Techno_Home.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using StoreDbContext = Techno_Home.Data.StoreDbContext;

namespace Techno_Home.Services
{
    public class ShoppingCartService
    {
        // Key used to store and retrieve the cart from session
        private const string SessionKey = "Cart";
        
        private readonly ISession _session;
        private readonly StoreDbContext _context;

        // Constructor initializes the session and database context
        public ShoppingCartService(IHttpContextAccessor accessor, StoreDbContext context)
        {
            _session = accessor.HttpContext.Session;
            _context = context;
        }
        
        
        // Retrieves the cart from the session.
        // If no cart exists, returns an empty list.
        public List<CartItem> GetCart()
        {
            var cartJson = _session.GetString(SessionKey);
            return string.IsNullOrEmpty(cartJson) ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(cartJson);
        }

        // Saves the current cart to the session by serializing it to JSON.
        private void SaveCart(List<CartItem> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            _session.SetString(SessionKey, cartJson);
        }

        // Adds a product to the cart.
        // If the item already exists, it increases the quantity.
        // If not, fetches the product from the database and adds it as a new item.
        public void AddToCart(int productId, int quantity = 1)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                var product = _context.Products.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    cart.Add(new CartItem
                    {
                        ProductId = productId,
                        Product = product,
                        Quantity = quantity
                    });
                }
            }
            
            SaveCart(cart);
        }

        // Removes all instances of a specific product from the cart.
        public void RemoveFromCart(int productId)
        {
            var cart = GetCart();
            cart.RemoveAll(i => i.ProductId == productId);
            SaveCart(cart);
        }

        // Updates the quantity of a product in the cart, if it exists.
        public void UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
            }
            SaveCart(cart);
        }

        // Returns the full list of items currently in the cart.
        public List<CartItem> GetCartItems()
        {
            return GetCart();
        }

        // Calculates the total price for all items in the cart.
        public decimal GetTotal()
        {
            return GetCart().Sum(i => (i.Product.Price ?? 0) * i.Quantity);
        }

        // Clears the cart by removing it from the session.
        public void ClearCart()
        {
            _session.Remove(SessionKey);
        }
    }
}