using MedishcMVCProject.DAL;
using MedishcMVCProject.Models;
using MedishcMVCProject.Models.Pharmacy;
using MedishcMVCProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Security.Claims;

namespace MedishcMVCProject.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public ShopController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            List<Product>? products = await _context.Products.ToListAsync();
            return View(products);
        }

        public IActionResult ProductDetail()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, string returnUrl)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "PharmacyAccount");

            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.ProductId == productId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += 1;
                _context.CartItems.Update(existingCartItem);
            }
            else
            {
                var newCartItem = new CartItem
                {
                    ProductId = productId,
                    UserId = user.Id,
                    Quantity = 1,
                };
                _context.CartItems.Add(newCartItem);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Məhsul səbətə əlavə olundu!";

            return Redirect(returnUrl ?? "/Shop");
        }

        public async Task<IActionResult> ViewCart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "PharmacyAccount");

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            var cartVMs = cartItems.Select(c => new CartItemVM
            {
                ProductId = c.ProductId,
                ProductName = c.Product.Name,
                ImageUrl = c.Product.ImageUrl,
                Price = c.Product.Price,
                Quantity = c.Quantity
            }).ToList();

            return View(cartVMs);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCartQuantity(int productId, int quantity)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "İstifadəçi tapılmadı" });

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == user.Id);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                _context.CartItems.Update(cartItem);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Miqdar yeniləndi" });
            }

            return Json(new { success = false, message = "Məhsul tapılmadı" });
        }


        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "İstifadəçi tapılmadı" });
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.ProductId == id && c.UserId == user.Id);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Məhsul silindi" });
            }

            return Json(new { success = false, message = "Məhsul tapılmadı" });
        }

        [HttpGet]
        public async Task<IActionResult> GetCartData()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { items = new List<object>() });
            }

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            var cartData = new
            {
                items = cartItems.Select(c => new
                {
                    name = c.Product.Name,
                    price = c.Product.Price,
                    quantity = c.Quantity,
                    productId = c.ProductId
                }).ToList()
            };

            return Json(cartData);
        }
        [HttpPost]
        public IActionResult ClearCart()
        {
            try
            {
                HttpContext.Session.Remove("Cart");

                if (User.Identity.IsAuthenticated)
                {
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var cartItems = _context.CartItems.Where(x => x.UserId == userId);
                        _context.CartItems.RemoveRange(cartItems);
                        _context.SaveChanges();
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "İstifadəçi tapılmadı. Zəhmət olmasa hesabınıza daxil olun.";
                return RedirectToAction("Login", "PharmacyAccount");
            }

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Səbət boşdur.";
                return RedirectToAction("ViewCart");
            }

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckoutSession()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            if (!cartItems.Any()) return BadRequest("Səbət boşdur.");

            var domain = $"{Request.Scheme}://{Request.Host}";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = cartItems.Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100),
                        Currency = "azn",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                            Description = item.Product.Description
                        }
                    },
                    Quantity = item.Quantity,
                }).ToList(),
                Mode = "payment",
                SuccessUrl = domain + Url.Action("PaymentSuccess", "Shop") + "?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = domain + Url.Action("PaymentCancel", "Shop"),
                Metadata = new Dictionary<string, string>
            {
                { "userId", user.Id }
            }
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Json(new { id = session.Id });
        }

        public async Task<IActionResult> PaymentSuccess(string session_id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var service = new SessionService();
            var session = service.Get(session_id);

            if (session.PaymentStatus != "paid")
            {
                TempData["Error"] = "Ödəniş tamamlanmayıb.";
                return RedirectToAction("Checkout");
            }

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Səbət boşdur.";
                return RedirectToAction("Checkout");
            }

            //using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    UserId = user.Id,
                    OrderDate = DateTime.Now,
                    TotalAmount = cartItems.Sum(i => i.Product.Price * i.Quantity),
                    Status = OrderStatus.Paid,
                    Items = cartItems.Select(c => new OrderItem
                    {
                        ProductId = c.ProductId,
                        Quantity = c.Quantity,
                        Price = c.Product.Price
                    }).ToList()
                };

                _context.Orders.Add(order);


                var payment = new Payment
                {
                    Order = order,
                    Amount = order.TotalAmount,
                    PaidAt = DateTime.Now,
                    Method = PaymentMethod.Stripe,
                    Status = PaymentStatus.Completed,
                    FailureReason = "tst"
                };

                _context.Payments.Add(payment);
                _context.CartItems.RemoveRange(cartItems);

                await _context.SaveChangesAsync();
                //await transaction.CommitAsync();

                TempData["Success"] = "Ödəniş uğurla tamamlandı. Sifarişiniz qəbul edildi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //await transaction.RollbackAsync();
                TempData["Error"] = "Ödəniş sonrası xəta baş verdi.";
                return RedirectToAction("Checkout");
            }
        }

        public IActionResult PaymentCancel()
        {
            TempData["Warning"] = "Ödəniş ləğv edildi.";
            return RedirectToAction("Checkout");
        }




    }
}
