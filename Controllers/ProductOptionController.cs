using Glowry.Data;
using Glowry.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Glowry.Controllers
{
    public class ProductOptionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductOptionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductOption
        public async Task<IActionResult> Index(int? productId)
        {
            var query = _context.ProductOptions
                .Include(po => po.Product)
                .AsQueryable();

            if (productId.HasValue)
                query = query.Where(po => po.ProId == productId.Value);

            ViewBag.Products = await _context.Products.ToListAsync();
            return View(await query.ToListAsync());
        }

        // GET: ProductOption/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var productOption = await _context.ProductOptions
                .Include(po => po.Product)
                .Include(po => po.ImageMaps)
                    .ThenInclude(im => im.ProductImg)
                .FirstOrDefaultAsync(m => m.OptionId == id);

            if (productOption == null)
                return NotFound();

            return View(productOption);
        }

        // GET: ProductOption/Create
        public IActionResult Create(int? productId)
        {
            ViewData["ProId"] = productId.HasValue
                ? new SelectList(_context.Products, "ProId", "ProName", productId.Value)
                : new SelectList(_context.Products, "ProId", "ProName");

            return View();
        }

        // POST: ProductOption/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OptionId,AttributeName,AttributeValue,ProId,StockQuantity")] ProductOption productOption)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(productOption);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageOptions", "Product", new { id = productOption.ProId });
            }
            ViewData["ProId"] = new SelectList(_context.Products, "ProId", "ProName", productOption.ProId);
            return View(productOption);
        }

        // GET: ProductOption/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var productOption = await _context.ProductOptions.FindAsync(id);
            if (productOption == null)
                return NotFound();

            ViewData["ProId"] = new SelectList(_context.Products, "ProId", "ProName", productOption.ProId);
            return View(productOption);
        }

        // POST: ProductOption/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OptionId,AttributeName,AttributeValue,ProId,StockQuantity")] ProductOption productOption)
        {
            if (id != productOption.OptionId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productOption);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductOptionExists(productOption.OptionId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("ManageOptions", "Product", new { id = productOption.ProId });
            }
            ViewData["ProId"] = new SelectList(_context.Products, "ProId", "ProName", productOption.ProId);
            return View(productOption);
        }

        // GET: ProductOption/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var productOption = await _context.ProductOptions
                .Include(po => po.Product)
                .FirstOrDefaultAsync(m => m.OptionId == id);

            if (productOption == null)
                return NotFound();

            return View(productOption);
        }

        // POST: ProductOption/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productOption = await _context.ProductOptions.FindAsync(id);
            int productId = productOption.ProId;

            if (productOption != null)
            {
                _context.ProductOptions.Remove(productOption);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ManageOptions", "Product", new { id = productId });
        }

        // GET: ProductOption/MapImages/5
        public async Task<IActionResult> MapImages(int? id)
        {
            if (id == null)
                return NotFound();

            var productOption = await _context.ProductOptions
                .Include(po => po.Product)
                    .ThenInclude(p => p.ProductImages)
                .Include(po => po.ImageMaps)
                    .ThenInclude(im => im.ProductImg)
                .FirstOrDefaultAsync(po => po.OptionId == id);

            if (productOption == null)
                return NotFound();

            // Get available images (product images not yet mapped to this option)
            var mappedImageIds = productOption.ImageMaps.Select(im => im.ImgId).ToList();
            ViewBag.AvailableImages = productOption.Product.ProductImages
                .Where(img => !mappedImageIds.Contains(img.ImgId))
                .ToList();

            return View(productOption);
        }

        private bool ProductOptionExists(int id)
        {
            return _context.ProductOptions.Any(e => e.OptionId == id);
        }
    }
}