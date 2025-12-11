using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameShoppingMvcUI.Repositories
{
    public class CartRepository : ICartRepository
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
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public async Task<bool> RemoveItem(int gameId)
        {
            try
            {
                string? userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return false;
                var cart = await GetCart(userId);
                if (cart is null)
                    return false;
                // cart details section
                var cartItem = await _db.CartDetails
                    .FirstOrDefaultAsync(u => u.ShoppingCartId == cart.Id && u.GameId == gameId);
                if (cartItem is null)
                {
                    return false;
                }
                //if quentity of an item 1 so remove it from cart
                else if (cartItem.Quantity == 1)
                {
                    _db.CartDetails.Remove(cartItem);

                }
                else
                {
                    cartItem.Quantity--;
                }
                _db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public async Task<IEnumerable<ShoppingCart>> GetUserCart()
        {
            string? userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("User not found");
            var cart = await _db.ShoppingCarts
                .Where(u => u.UserId == userId)
                .Include(u => u.CartDetails!)
                .ThenInclude(u => u.Game)
                .ToListAsync();
            return cart;

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