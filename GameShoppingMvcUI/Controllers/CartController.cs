using Microsoft.AspNetCore.Mvc;

namespace GameShoppingMvcUI.Controllers
{
    public class CartController : Controller
    {
        public readonly ICartRepository _cartRepo;

        public CartController(ICartRepository cartRepo) 
        {
            _cartRepo = cartRepo;
        } 
        public async Task<IActionResult> AddItem(int gameId, int qty = 1, int redirect = 0)
        {
            var cartCount = await _cartRepo.AddItem(gameId, qty);
            if (redirect == 0)
                return Ok(cartCount);
            return RedirectToAction("GetUserCart");
        }
        public async Task<IActionResult> RemoveItem(int gameId)
        {
            var cartCount = await _cartRepo.RemoveItem(gameId);
            return RedirectToAction("GetUserCart");
        }
        public async Task<IActionResult> GetUserCart()
        {
            var cart = await _cartRepo.GetUserCart();
            return View(cart);
        }
        public async Task<IActionResult> GetTotalItemInCart()
        {
            int? cartItem = await _cartRepo.GetTotalItemCart();
            return Ok(cartItem);
        }
        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(string fullName, string email, string phone, string address)
        {
            // This is a dummy implementation for testing
            // In a real scenario, you would create an order and process the cart
            bool isCheckout = await _cartRepo.DoCheckout();
            if (!isCheckout)
            {
                TempData["ErrorMessage"] = "Checkout failed. Please try again.";
                return RedirectToAction(nameof(Checkout));
            }

            // Pass the customer details to the success page
            TempData["CustomerName"] = fullName;
            TempData["OrderEmail"] = email;
            return RedirectToAction("OrderSuccess");
        }

        public IActionResult OrderSuccess()
        {
            ViewBag.CustomerName = TempData["CustomerName"] ?? "Customer";
            ViewBag.OrderEmail = TempData["OrderEmail"] ?? "";
            return View();
        }
    }
}
