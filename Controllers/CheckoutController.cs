using AkimWigs.Web.Models;
using AkimWigs.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace AkimWigs.Web.Controllers
{
    public class CheckoutController : Controller
    {
        private const string CartSessionKey = "Cart";

        // GET: /Checkout
        [HttpGet]
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey)
                       ?? new List<CartItem>();

            if (!cart.Any())
            {
                // Rien dans le panier -> retour à la boutique
                return RedirectToAction("Index", "Product");
            }

            decimal subtotal = cart.Sum(i => i.Total);

            // Récupérer la livraison depuis la session
            decimal delivery = 0;
            var deliveryStr = HttpContext.Session.GetString("Delivery");
            if (!string.IsNullOrEmpty(deliveryStr) && decimal.TryParse(deliveryStr, out var d))
            {
                delivery = d;
            }
            else
            {
                delivery = 12; // par défaut : standard
            }

            decimal taxRate = 0.15m; // TVH 15% NB
            decimal taxes = (subtotal + delivery) * taxRate;
            decimal total = subtotal + delivery + taxes;

            var model = new CheckoutViewModel
            {
                Subtotal = subtotal,
                Delivery = delivery,
                Taxes = taxes,
                Total = total,
                Country = "Canada"
            };

            return View(model);
        }

        // POST: /Checkout (création session Stripe)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CheckoutViewModel model)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey)
                       ?? new List<CartItem>();

            if (!cart.Any())
            {
                ModelState.AddModelError(string.Empty, "Votre panier est vide.");
            }

            // Recalculer les montants côté serveur
            decimal subtotal = cart.Sum(i => i.Total);

            decimal delivery = 0;
            var deliveryStr = HttpContext.Session.GetString("Delivery");
            if (!string.IsNullOrEmpty(deliveryStr) && decimal.TryParse(deliveryStr, out var d))
            {
                delivery = d;
            }
            else
            {
                delivery = 12;
            }

            decimal taxRate = 0.15m;
            decimal taxes = (subtotal + delivery) * taxRate;
            decimal total = subtotal + delivery + taxes;

            model.Subtotal = subtotal;
            model.Delivery = delivery;
            model.Taxes = taxes;
            model.Total = total;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Création session Stripe Checkout
            var domain = $"{Request.Scheme}://{Request.Host}";

            var options = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = domain + Url.Action("Success"),
                CancelUrl = domain + Url.Action("Cancel"),
                CustomerEmail = model.Email,
                LineItems = new List<SessionLineItemOptions>()
            };

            // On met un seul line item "Commande AkimWigs" avec le total TTC
            options.LineItems.Add(new SessionLineItemOptions
            {
                Quantity = 1,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "cad",
                    UnitAmount = (long)(total * 100), // en cents
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Commande AkimWigs"
                    }
                }
            });

            var service = new SessionService();
            Session session = service.Create(options);

            // On pourrait stocker quelques infos de checkout en session pour plus tard (mail, adresse...)
            HttpContext.Session.SetString("LastStripeSessionId", session.Id);

            return Redirect(session.Url);
        }

        // GET: /Checkout/Success
        public IActionResult Success()
        {
            // On vide le panier
            HttpContext.Session.Remove(CartSessionKey);

            return View();
        }

        // GET: /Checkout/Cancel
        public IActionResult Cancel()
        {
            return View();
        }
    }
}
