using Glowry.Data;
using Glowry.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Glowry.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Home/Index
        public async Task<IActionResult> Index()
        {
            // Get featured products (top 4 or 8)
            var featuredProducts = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductOptions)
                .Where(p => p.isfeatured)
                .OrderByDescending(p => p.CreatedAt)
                .Take(8)
                .ToListAsync();

            // Get categories with product counts
            var categories = await _context.Categories
                .Include(c => c.Products)
                .OrderBy(c => c.CategName)
                .ToListAsync();

            // Get statistics
            var stats = new
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalCustomers = await _context.Users.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync(),
                FeaturedProducts = featuredProducts.Count
            };

            ViewBag.FeaturedProducts = featuredProducts;
            ViewBag.Categories = categories;
            ViewBag.Stats = stats;

            return View();
        }

        // GET: Home/About
        public IActionResult About()
        {
            return View();
        }

        // GET: Home/Contact
        public IActionResult Contact()
        {
            return View();
        }

        // POST: Home/Contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(string name, string email, string subject, string message)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(message))
            {
                TempData["Error"] = "Please fill in all required fields.";
                return View();
            }

            // TODO: Send email or save to database
            // For now, just show success message
            TempData["Success"] = "Thank you for contacting us! We'll get back to you soon.";
            return RedirectToAction(nameof(Contact));
        }

        // POST: Home/Subscribe (Newsletter)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Please enter a valid email address." });
            }

            // TODO: Save to newsletter subscribers table
            // For now, just return success
            return Json(new { success = true, message = "Thank you for subscribing!" });
        }

        // GET: Home/Privacy
        public IActionResult Privacy()
        {
            return View();
        }

        // GET: Home/Terms
        public IActionResult Terms()
        {
            return View();
        }

        // GET: Home/Search
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return RedirectToAction(nameof(Index));
            }

            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.ProName.Contains(query) ||
                           p.ProDescription.Contains(query) ||
                           p.Category.CategName.Contains(query))
                .ToListAsync();

            ViewBag.SearchQuery = query;
            ViewBag.ResultCount = products.Count;

            return View(products);
        }

        // GET: Home/Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}