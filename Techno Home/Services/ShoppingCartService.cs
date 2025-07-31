using Techno_Home.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Techno_Home.Data;

namespace Techno_Home.Services
{
    public class ShoppingCartService
    {
        private const string SessionKey = "Cart";
        private readonly ISession _session;
        private readonly StoreDbContext _context;

        public ShoppingCartService(IHttpContextAccessor accessor, StoreDbContext context)
        {
            _session = accessor.HttpContext.Session;
            _context = context;
        }
        
        public List<CartItem> GetCart()
        {
            var cartJson = _session.GetString(SessionKey);
            return string.IsNullOrEmpty(cartJson) ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(cartJson);
        }

        private void SaveCart(List<CartItem> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            _session.SetString(SessionKey, cartJson);
        }

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

        public void RemoveFromCart(int productId)
        {
            var cart = GetCart();
            cart.RemoveAll(i => i.ProductId == productId);
            SaveCart(cart);
        }

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

        public List<CartItem> GetCartItems()
        {
            return GetCart();
        }

        public decimal GetTotal()
        {
            return GetCart().Sum(i => (i.Product.Price ?? 0) * i.Quantity);
        }

        public void ClearCart()
        {
            _session.Remove(SessionKey);
        }
    }
}