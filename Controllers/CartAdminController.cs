using Glowry.Data;
using Glowry.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Glowry.Controllers
{
    
    public class CartAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CartAdmin
        public async Task<IActionResult> Index(string searchUser)
        {
            var query = _context.Carts
                .Include(c => c.AppUser)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductOption)
                        .ThenInclude(po => po.Product)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchUser))
            {
                query = query.Where(c =>
                    c.AppUser.UserName.Contains(searchUser) ||
                    c.AppUser.Email.Contains(searchUser));
            }

            var carts = await query.ToListAsync();
            ViewBag.SearchUser = searchUser;

            return View(carts);
        }

        // GET: CartAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var cart = await _context.Carts
                .Include(c => c.AppUser)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductOption)
                        .ThenInclude(po => po.Product)
                            .ThenInclude(p => p.ProductImages)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductOption)
                        .ThenInclude(po => po.ImageMaps)
                            .ThenInclude(im => im.ProductImg)
                .FirstOrDefaultAsync(m => m.CartId == id);

            if (cart == null)
                return NotFound();

            return View(cart);
        }

        // GET: CartAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var cart = await _context.Carts
                .Include(c => c.AppUser)
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(m => m.CartId == id);

            if (cart == null)
                return NotFound();

            return View(cart);
        }

        // POST: CartAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CartId == id);

            if (cart != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cart deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: CartAdmin/ClearCart/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart(int id)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CartId == id);

            if (cart != null && cart.CartItems.Any())
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cart cleared successfully.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: CartAdmin/RemoveItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int cartItemId, int cartId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Item removed from cart.";
            }

            return RedirectToAction(nameof(Details), new { id = cartId });
        }

        // GET: CartAdmin/AbandonedCarts
        public async Task<IActionResult> AbandonedCarts(int days = 7)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);

            // You'll need to add a LastModified field to Cart model for this to work properly
            // For now, we'll just show all non-empty carts
            var carts = await _context.Carts
                .Include(c => c.AppUser)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductOption)
                        .ThenInclude(po => po.Product)
                .Where(c => c.CartItems.Any())
                .ToListAsync();

            ViewBag.Days = days;
            return View(carts);
        }

        // GET: CartAdmin/Statistics
        public async Task<IActionResult> Statistics()
        {
            var totalCarts = await _context.Carts.CountAsync();
            var activeCarts = await _context.Carts.CountAsync(c => c.CartItems.Any());
            var emptyCarts = totalCarts - activeCarts;
            var totalItems = await _context.CartItems.SumAsync(ci => ci.Quantity);

            var cartValues = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductOption)
                        .ThenInclude(po => po.Product)
                .Select(c => new
                {
                    CartId = c.CartId,
                    Total = c.CartItems.Sum(ci => ci.Quantity * ci.ProductOption.Product.Price)
                })
                .ToListAsync();

            var totalValue = cartValues.Sum(cv => cv.Total);
            var averageValue = activeCarts > 0 ? totalValue / activeCarts : 0;

            ViewBag.TotalCarts = totalCarts;
            ViewBag.ActiveCarts = activeCarts;
            ViewBag.EmptyCarts = emptyCarts;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalValue = totalValue;
            ViewBag.AverageValue = averageValue;

            return View();
        }
    }
}