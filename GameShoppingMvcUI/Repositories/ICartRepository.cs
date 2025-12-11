namespace GameShoppingMvcUI.Repositories
{
    public interface ICartRepository
    {
        Task<int> AddItem(int gameId, int qty);
        Task<int> RemoveItem(int gameId);
        Task<IEnumerable<ShoppingCart>> GetUserCart();
        Task<int?> GetCartItemCount(string userId = "");
    }
}
