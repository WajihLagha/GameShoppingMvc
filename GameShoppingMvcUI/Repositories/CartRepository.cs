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
        public async Task<int> AddItem(int gameId, int qty)
        {
            string? userId = GetUserId();
            var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // add cart if not exists
                if (userId == null)
                {
                    throw new Exception("User is not logged in");
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
                Console.WriteLine(e.Message);
                transaction.Rollback();
            }
            int? cartItemCount = await GetTotalItemCart(userId);
            return cartItemCount ?? 00;

        }
        public async Task<int> RemoveItem(int gameId)
        {
            string? userId = GetUserId();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User not found");
                var cart = await GetCart(userId);
                if (cart is null)
                    throw new Exception("Cart not found");
                // cart details section
                var cartItem = await _db.CartDetails
                    .FirstOrDefaultAsync(u => u.ShoppingCartId == cart.Id && u.GameId == gameId);
                if (cartItem is null)
                {
                    throw new Exception("Cart item not found");
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
            }
            catch (Exception e)
            {
                
            }
            int? cartItemCount = await GetTotalItemCart(userId);
            return cartItemCount ?? 00;
        }
        public async Task<ShoppingCart> GetUserCart()
        {
            string? userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("User not found");
            var cart = await _db.ShoppingCarts
                .Where(u => u.UserId == userId)
                .Include(u => u.CartDetails!)
                .ThenInclude(u => u.Game)
                .FirstOrDefaultAsync();
            return cart;

        }

        public async Task<ShoppingCart?> GetCart(string userId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(u => u.UserId == userId);
            return cart;
        }
        public async Task<int?> GetTotalItemCart(string userId="")
        {
            if(string.IsNullOrEmpty(userId))
            {
                userId = GetUserId() ?? "";
            }
            var data = await (from cart in _db.ShoppingCarts
                              join cartDetails in _db.CartDetails
                              on cart.Id equals cartDetails.ShoppingCartId
                              where cart.UserId == userId
                              select new { cartDetails.Id}
                              ).ToListAsync();
            return data.Count;
        }

        private string? GetUserId()
        {
            var principal = _httpContextAccessor?.HttpContext?.User;
            string? userId = _userManager.GetUserId(principal);
            return userId;

        }
    }
}