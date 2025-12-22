using Glowry.Data;
using Glowry.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Glowry.Controllers
{
    public class ProductImgController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductImgController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductImg
        public async Task<IActionResult> Index(int? productId)
        {
            var query = _context.ProductImgs
                .Include(pi => pi.Product)
                .AsQueryable();

            if (productId.HasValue)
                query = query.Where(pi => pi.ProductId == productId.Value);

            ViewBag.Products = await _context.Products.ToListAsync();
            return View(await query.ToListAsync());
        }

        // GET: ProductImg/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var productImg = await _context.ProductImgs
                .Include(pi => pi.Product)
                .Include(pi => pi.ImageMaps)
                    .ThenInclude(im => im.ProductOption)
                .FirstOrDefaultAsync(m => m.ImgId == id);

            if (productImg == null)
                return NotFound();

            return View(productImg);
        }

        // GET: ProductImg/Create
        public IActionResult Create(int? productId)
        {
            ViewData["ProductId"] = productId.HasValue
                ? new SelectList(_context.Products, "ProId", "ProName", productId.Value)
                : new SelectList(_context.Products, "ProId", "ProName");

            return View();
        }

        // POST: ProductImg/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile imageFile, string alt, int? productId)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                ModelState.AddModelError("", "Please select an image file");
                ViewData["ProductId"] = new SelectList(_context.Products, "ProId", "ProName", productId);
                return View();
            }

            byte[] imageData;
            using (var ms = new MemoryStream())
            {
                await imageFile.CopyToAsync(ms);
                imageData = ms.ToArray();
            }

            var productImg = new ProductImg
            {
                Img = imageData,
                Alt = alt,
                ProductId = productId
            };

            _context.Add(productImg);
            await _context.SaveChangesAsync();

            if (productId.HasValue)
                return RedirectToAction("ManageImages", "Product", new { id = productId.Value });

            return RedirectToAction(nameof(Index));
        }

        // POST: ProductImg/CreateMultiple
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMultiple(List<IFormFile> imageFiles, string alt, int? productId)
        {
            if (imageFiles == null || imageFiles.Count == 0)
            {
                ModelState.AddModelError("", "Please select at least one image file");
                return RedirectToAction("ManageImages", "Product", new { id = productId });
            }

            foreach (var file in imageFiles)
            {
                if (file.Length > 0)
                {
                    byte[] imageData;
                    using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        imageData = ms.ToArray();
                    }

                    var productImg = new ProductImg
                    {
                        Img = imageData,
                        Alt = alt ?? file.FileName,
                        ProductId = productId
                    };

                    _context.Add(productImg);
                }
            }

            await _context.SaveChangesAsync();

            if (productId.HasValue)
                return RedirectToAction("ManageImages", "Product", new { id = productId.Value });

            return RedirectToAction(nameof(Index));
        }

        // GET: ProductImg/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var productImg = await _context.ProductImgs.FindAsync(id);
            if (productImg == null)
                return NotFound();

            ViewData["ProductId"] = new SelectList(_context.Products, "ProId", "ProName", productImg.ProductId);
            return View(productImg);
        }

        // POST: ProductImg/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile imageFile, string alt, int? productId)
        {
            var productImg = await _context.ProductImgs.FindAsync(id);
            if (productImg == null)
                return NotFound();

            if (imageFile != null && imageFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await imageFile.CopyToAsync(ms);
                    productImg.Img = ms.ToArray();
                }
            }

            productImg.Alt = alt;
            productImg.ProductId = productId;

            try
            {
                _context.Update(productImg);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductImgExists(productImg.ImgId))
                    return NotFound();
                else
                    throw;
            }

            if (productId.HasValue)
                return RedirectToAction("ManageImages", "Product", new { id = productId.Value });

            return RedirectToAction(nameof(Index));
        }

        // GET: ProductImg/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var productImg = await _context.ProductImgs
                .Include(pi => pi.Product)
                .FirstOrDefaultAsync(m => m.ImgId == id);

            if (productImg == null)
                return NotFound();

            return View(productImg);
        }

        // POST: ProductImg/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productImg = await _context.ProductImgs.FindAsync(id);
            int? productId = productImg?.ProductId;

            if (productImg != null)
            {
                _context.ProductImgs.Remove(productImg);
                await _context.SaveChangesAsync();
            }

            if (productId.HasValue)
                return RedirectToAction("ManageImages", "Product", new { id = productId.Value });

            return RedirectToAction(nameof(Index));
        }

        // GET: ProductImg/GetImage/5
        public async Task<IActionResult> GetImage(int id)
        {
            var productImg = await _context.ProductImgs.FindAsync(id);

            if (productImg == null || productImg.Img == null)
                return NotFound();

            return File(productImg.Img, "image/jpeg");
        }

        private bool ProductImgExists(int id)
        {
            return _context.ProductImgs.Any(e => e.ImgId == id);
        }
    }
}