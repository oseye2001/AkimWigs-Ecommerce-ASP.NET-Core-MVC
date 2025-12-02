using AkimWigs.Infrastructure.Data;
using AkimWigs.Web.Extensions;
using AkimWigs.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AkimWigs.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CartSessionKey = "Cart";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Cart
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey)
                       ?? new List<CartItem>();

            return View(cart);
        }

        // POST: /Cart/Add
        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
                return NotFound();

            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey)
                       ?? new List<CartItem>();

            var existing = cart.FirstOrDefault(c => c.ProductId == productId);
            if (existing != null)
            {
                existing.Quantity += 1;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    UnitPrice = product.Price,
                    Quantity = 1,
                    ImageUrl = product.ImageUrl ?? string.Empty
                });
            }

            HttpContext.Session.SetObject(CartSessionKey, cart);

            // Retour vers la boutique
            return RedirectToAction("Index", "Product");
        }

        // POST: /Cart/Remove
        [HttpPost]
        public IActionResult Remove(int productId)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey)
                       ?? new List<CartItem>();

            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.SetObject(CartSessionKey, cart);
            }

            return RedirectToAction("Index");
        }

        // POST: /Cart/UpdateQuantity
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey)
                       ?? new List<CartItem>();

            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity <= 0 ? 1 : quantity;
                HttpContext.Session.SetObject(CartSessionKey, cart);
            }

            return RedirectToAction("Index");
        }

        // POST: /Cart/SetDelivery
        [HttpPost]
        public IActionResult SetDelivery(decimal delivery)
        {
            HttpContext.Session.SetString("Delivery", delivery.ToString());
            return RedirectToAction("Index");
        }
    }
}
