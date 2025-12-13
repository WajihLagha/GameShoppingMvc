namespace GameShoppingMvcUI.Repositories
{
    public interface ICartRepository
    {
        Task<int> AddItem(int gameId, int qty);
        Task<int> RemoveItem(int gameId);
        Task<ShoppingCart> GetUserCart();
        Task<int?> GetTotalItemCart(string userId = "");
    }
}
