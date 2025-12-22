using Glowry.Data;
using Glowry.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Glowry.Controllers
{
    public class ImageMapController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ImageMapController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ImageMap
        public async Task<IActionResult> Index()
        {
            var imageMaps = await _context.ImageMaps
                .Include(im => im.ProductImg)
                    .ThenInclude(img => img.Product)
                .Include(im => im.ProductOption)
                    .ThenInclude(po => po.Product)
                .ToListAsync();

            return View(imageMaps);
        }

        // GET: ImageMap/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var imageMap = await _context.ImageMaps
                .Include(im => im.ProductImg)
                .Include(im => im.ProductOption)
                .FirstOrDefaultAsync(m => m.ImgMapId == id);

            if (imageMap == null)
                return NotFound();

            return View(imageMap);
        }

        // GET: ImageMap/Create
        public IActionResult Create(int? optionId, int? imageId)
        {
            if (optionId.HasValue)
            {
                var option = _context.ProductOptions
                    .Include(po => po.Product)
                        .ThenInclude(p => p.ProductImages)
                    .FirstOrDefault(po => po.OptionId == optionId.Value);

                if (option != null)
                {
                    ViewData["ImgId"] = new SelectList(option.Product.ProductImages, "ImgId", "Alt", imageId);
                    ViewData["OptionId"] = new SelectList(new[] { option }, "OptionId", "AttributeValue", optionId.Value);
                    return View();
                }
            }

            ViewData["ImgId"] = new SelectList(_context.ProductImgs, "ImgId", "Alt", imageId);
            ViewData["OptionId"] = new SelectList(_context.ProductOptions, "OptionId", "AttributeValue", optionId);
            return View();
        }

        // POST: ImageMap/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ImgMapId,ImgId,OptionId")] ImageMap imageMap)
        {
            // Check if mapping already exists
            var existingMap = await _context.ImageMaps
                .FirstOrDefaultAsync(im => im.ImgId == imageMap.ImgId && im.OptionId == imageMap.OptionId);

            if (existingMap != null)
            {
                ModelState.AddModelError("", "This image is already mapped to this option");
                ViewData["ImgId"] = new SelectList(_context.ProductImgs, "ImgId", "Alt", imageMap.ImgId);
                ViewData["OptionId"] = new SelectList(_context.ProductOptions, "OptionId", "AttributeValue", imageMap.OptionId);
                return View(imageMap);
            }

            if (ModelState.IsValid)
            {
                _context.Add(imageMap);
                await _context.SaveChangesAsync();
                return RedirectToAction("MapImages", "ProductOption", new { id = imageMap.OptionId });
            }

            ViewData["ImgId"] = new SelectList(_context.ProductImgs, "ImgId", "Alt", imageMap.ImgId);
            ViewData["OptionId"] = new SelectList(_context.ProductOptions, "OptionId", "AttributeValue", imageMap.OptionId);
            return View(imageMap);
        }

        // POST: ImageMap/CreateMultiple
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMultiple(int optionId, List<int> imageIds)
        {
            if (imageIds == null || !imageIds.Any())
            {
                TempData["Error"] = "Please select at least one image";
                return RedirectToAction("MapImages", "ProductOption", new { id = optionId });
            }

            foreach (var imageId in imageIds)
            {
                // Check if mapping already exists
                var existingMap = await _context.ImageMaps
                    .FirstOrDefaultAsync(im => im.ImgId == imageId && im.OptionId == optionId);

                if (existingMap == null)
                {
                    var imageMap = new ImageMap
                    {
                        ImgId = imageId,
                        OptionId = optionId
                    };
                    _context.Add(imageMap);
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Images mapped successfully";
            return RedirectToAction("MapImages", "ProductOption", new { id = optionId });
        }

        // GET: ImageMap/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var imageMap = await _context.ImageMaps
                .Include(im => im.ProductImg)
                .Include(im => im.ProductOption)
                .FirstOrDefaultAsync(m => m.ImgMapId == id);

            if (imageMap == null)
                return NotFound();

            return View(imageMap);
        }

        // POST: ImageMap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var imageMap = await _context.ImageMaps.FindAsync(id);
            int optionId = imageMap.OptionId;

            if (imageMap != null)
            {
                _context.ImageMaps.Remove(imageMap);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MapImages", "ProductOption", new { id = optionId });
        }

        // POST: ImageMap/DeleteByIds
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteByIds(int optionId, int imageId)
        {
            var imageMap = await _context.ImageMaps
                .FirstOrDefaultAsync(im => im.OptionId == optionId && im.ImgId == imageId);

            if (imageMap != null)
            {
                _context.ImageMaps.Remove(imageMap);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MapImages", "ProductOption", new { id = optionId });
        }

        // POST: ImageMap/DeleteAllForOption
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllForOption(int optionId)
        {
            var imageMaps = await _context.ImageMaps
                .Where(im => im.OptionId == optionId)
                .ToListAsync();

            if (imageMaps.Any())
            {
                _context.ImageMaps.RemoveRange(imageMaps);
                await _context.SaveChangesAsync();
                TempData["Success"] = "All image mappings removed";
            }

            return RedirectToAction("MapImages", "ProductOption", new { id = optionId });
        }

        private bool ImageMapExists(int id)
        {
            return _context.ImageMaps.Any(e => e.ImgMapId == id);
        }
    }
}