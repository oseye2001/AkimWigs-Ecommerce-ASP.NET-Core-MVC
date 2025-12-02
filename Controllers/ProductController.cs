using AkimWigs.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace AkimWigs.Web.Controllers   // même namespace que HomeController
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public ProductController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET: /Product
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                                         .Include(p => p.Category)
                                         .ToListAsync();

            return View(products);
        }

        // GET: /Product/Details/5
        // GET: /Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                                        .Include(p => p.Category)
                                        .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            // Produits de la même catégorie pour "Tu pourrais aussi aimer"
            var related = await _context.Products
                                        .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
                                        .Take(3)
                                        .ToListAsync();

            ViewBag.RelatedProducts = related;

            return View(product);
        }

        // POST: /Product/Contact (formulaire question sur longueur / texture)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(string fullName, string email, string message)
        {
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(message))
            {
                TempData["ContactSuccess"] = "Merci de remplir tous les champs du formulaire.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                SendContactEmail(fullName, email, message);
                TempData["ContactSuccess"] = "Merci pour ton message ! Akim te répondra rapidement.";
            }
            catch
            {
                TempData["ContactSuccess"] = "Ton message n'a pas pu être envoyé. Tu peux aussi contacter Akim directement par WhatsApp/Instagram.";
            }

            return RedirectToAction(nameof(Index));
        }

        private void SendContactEmail(string fullName, string email, string message)
        {
            var smtpHost = _config["Smtp:Host"];
            var smtpPort = int.Parse(_config["Smtp:Port"] ?? "587");
            var smtpUser = _config["Smtp:User"];
            var smtpPass = _config["Smtp:Password"];
            var from = _config["Smtp:From"];
            var to = _config["Smtp:To"] ?? from;

            var subject = $"[Contact AkimWigs] Question de {fullName}";
            var body = $@"
                <h2>Nouveau message depuis le formulaire AkimWigs</h2>
                <ul>
                    <li><strong>Nom :</strong> {fullName}</li>
                    <li><strong>Email :</strong> {email}</li>
                </ul>
                <h3>Question</h3>
                <p>{System.Net.WebUtility.HtmlEncode(message).Replace("\n", "<br/>")}</p>
            ";

            var mail = new MailMessage
            {
                From = new MailAddress(from, "Formulaire AkimWigs"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mail.To.Add(to);

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUser, smtpPass)
            };

            client.Send(mail);
        }
    }
}
