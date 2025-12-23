using Glowry.Data;
using Glowry.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Glowry.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductOption)
                        .ThenInclude(po => po.Product)
                            .ThenInclude(p => p.ProductImages)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductOption)
                        .ThenInclude(po => po.ImageMaps)
                            .ThenInclude(im => im.ProductImg)
                .FirstOrDefaultAsync(c => c.AppUserId == user.Id);

            if (cart == null)
            {
                // Create cart if doesn't exist
                cart = new Cart { AppUserId = user.Id };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return View(cart);
        }

        // POST: Cart/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int optionId, int quantity = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // Get or create cart
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.AppUserId == user.Id);

            if (cart == null)
            {
                cart = new Cart { AppUserId = user.Id };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Check if option exists and has stock
            var option = await _context.ProductOptions.FindAsync(optionId);
            if (option == null)
            {
                TempData["Error"] = "Product option not found.";
                return RedirectToAction("Index", "Product");
            }

            if (option.StockQuantity < quantity)
            {
                TempData["Error"] = $"Insufficient stock. Only {option.StockQuantity} available.";
                return RedirectToAction("Details", "Product", new { id = option.ProId });
            }

            // Check if item already in cart
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.OptionId == optionId);
            if (existingItem != null)
            {
                // Check if new quantity exceeds stock
                if (existingItem.Quantity + quantity > option.StockQuantity)
                {
                    TempData["Error"] = $"Cannot add more. Maximum stock available: {option.StockQuantity}";
                    return RedirectToAction(nameof(Index));
                }

                existingItem.Quantity += quantity;
                _context.Update(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    OptionId = optionId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Item added to cart!";

            return RedirectToAction(nameof(Index));
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "User not found" });

            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .Include(ci => ci.ProductOption)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.Cart.AppUserId == user.Id);

            if (cartItem == null)
                return Json(new { success = false, message = "Cart item not found" });

            if (quantity <= 0)
                return Json(new { success = false, message = "Quantity must be greater than 0" });

            if (quantity > cartItem.ProductOption.StockQuantity)
                return Json(new { success = false, message = $"Insufficient stock. Maximum: {cartItem.ProductOption.StockQuantity}" });

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();

            // Calculate new totals
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductOption)
                        .ThenInclude(po => po.Product)
                .FirstOrDefaultAsync(c => c.AppUserId == user.Id);

            var itemTotal = cartItem.ProductOption.Product.Price * quantity;
            var cartTotal = cart.CartItems.Sum(ci => ci.ProductOption.Product.Price * ci.Quantity);

            return Json(new
            {
                success = true,
                itemTotal = itemTotal.ToString("F2"),
                cartTotal = cartTotal.ToString("F2")
            });
        }

        // POST: Cart/RemoveItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.Cart.AppUserId == user.Id);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Item removed from cart.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Cart/Clear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.AppUserId == user.Id);

            if (cart != null && cart.CartItems.Any())
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cart cleared.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Cart/GetCartCount (for navbar badge)
        public async Task<IActionResult> GetCartCount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { count = 0 });

            var count = await _context.CartItems
                .Where(ci => ci.Cart.AppUserId == user.Id)
                .SumAsync(ci => ci.Quantity);

            return Json(new { count });
        }
    }
}