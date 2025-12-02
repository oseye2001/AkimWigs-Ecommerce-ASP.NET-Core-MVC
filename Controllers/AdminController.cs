using AkimWigs.Core.Models;
using AkimWigs.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AkimWigs.Web.Controllers
{
    [Authorize(
      AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme,
      Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin
        public async Task<IActionResult> Index(string? search, string? category)
        {
            var query = _context.Products
                                .Include(p => p.Category)
                                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(p =>
                    p.Name.Contains(search) ||
                    p.Texture.Contains(search) ||
                    p.Color.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(category) && category != "all")
            {
                query = query.Where(p => p.Category != null &&
                                         p.Category.Name == category);
            }

            var products = await query
                .OrderBy(p => p.Category!.Name)
                .ThenBy(p => p.Name)
                .ToListAsync();

            var categories = await _context.Categories
                                           .OrderBy(c => c.Name)
                                           .ToListAsync();

            ViewBag.Categories = categories;
            ViewBag.Search = search;
            ViewBag.SelectedCategory = category;

            return View(products);
        }

        // GET: /Admin/Create
        public async Task<IActionResult> Create()
        {
            await LoadCategories();
            return View(new Product());
        }

        // POST: /Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategories();
                return View(product);
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            await LoadCategories();
            return View(product);
        }

        // POST: /Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await LoadCategories();
                return View(product);
            }

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCategories()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewBag.Categories = new SelectList(categories, "Id", "Name");
        }
    }
}
