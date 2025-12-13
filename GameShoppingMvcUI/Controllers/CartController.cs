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
    }
}
