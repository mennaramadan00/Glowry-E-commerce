using Glowry.Data;
using Glowry.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Glowry.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .ToListAsync();

            return View(categories);
        }

        // GET: Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _context.Categories
                .Include(c => c.Products)
                    .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(m => m.CategoryId == id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile imageFile, string categName, string categSlug)
        {
            if (string.IsNullOrEmpty(categName))
            {
                ModelState.AddModelError("categName", "Category name is required");
                return View();
            }

            if (string.IsNullOrEmpty(categSlug))
            {
                ModelState.AddModelError("categSlug", "Category slug is required");
                return View();
            }

            // Check if slug already exists
            if (await _context.Categories.AnyAsync(c => c.CategSlug == categSlug))
            {
                ModelState.AddModelError("categSlug", "This slug already exists");
                return View();
            }

            byte[] imageData = null;
            if (imageFile != null && imageFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await imageFile.CopyToAsync(ms);
                    imageData = ms.ToArray();
                }
            }

            var category = new Category
            {
                CategName = categName,
                CategSlug = categSlug,
                Img = imageData
            };

            _context.Add(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Category created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile imageFile, string categName, string categSlug)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            if (string.IsNullOrEmpty(categName))
            {
                ModelState.AddModelError("categName", "Category name is required");
                return View(category);
            }

            if (string.IsNullOrEmpty(categSlug))
            {
                ModelState.AddModelError("categSlug", "Category slug is required");
                return View(category);
            }

            // Check if slug already exists (excluding current category)
            if (await _context.Categories.AnyAsync(c => c.CategSlug == categSlug && c.CategoryId != id))
            {
                ModelState.AddModelError("categSlug", "This slug already exists");
                return View(category);
            }

            category.CategName = categName;
            category.CategSlug = categSlug;

            if (imageFile != null && imageFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await imageFile.CopyToAsync(ms);
                    category.Img = ms.ToArray();
                }
            }

            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.CategoryId))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(m => m.CategoryId == id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
                return NotFound();

            // Check if category has products
            if (category.Products != null && category.Products.Any())
            {
                TempData["Error"] = $"Cannot delete category '{category.CategName}' because it has {category.Products.Count} product(s) assigned to it. Please reassign or delete the products first.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Category deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Category/GetImage/5
        public async Task<IActionResult> GetImage(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null || category.Img == null)
                return NotFound();

            return File(category.Img, "image/jpeg");
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}