using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameShoppingMvcUI.Repositories
{
    public class CartRepository
    {
        public readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public CartRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        public async Task<bool> AddItem(int gameId, int qty)
        {
            var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // add cart if not exists
                string? userId = GetUserId();
                if (userId == null)
                {
                    return false;
                }
                var cart = await GetCart(userId);
                if (cart == null)
                {
                    cart = new ShoppingCart()
                    {
                        UserId = userId,
                    };
                    _db.ShoppingCarts.Add(cart);
                }
                _db.SaveChanges();
                // cart details section
                var cartItem = await _db.CartDetails
                    .FirstOrDefaultAsync(u => u.ShoppingCartId == cart.Id && u.GameId == gameId);
                if (cartItem is not null)
                {
                    cartItem.Quantity += qty;

                }
                else
                {
                    cartItem = new CartDetail()
                    {
                        ShoppingCartId = cart.Id,
                        GameId = gameId,
                        Quantity = qty,
                    };
                    _db.CartDetails.Add(cartItem);
                }
                _db.SaveChanges();
                transaction.Commit();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        public void RemoveFromCart()
        {
            string? userId = GetUserId();
        }
        private async Task<ShoppingCart?> GetCart(string userId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(u => u.UserId == userId);
            return cart;
        }
        
        private string? GetUserId()
        {
            var principal = _httpContextAccessor?.HttpContext?.User;
            string? userId = _userManager.GetUserId(principal);
            return userId;

        }
    }
}