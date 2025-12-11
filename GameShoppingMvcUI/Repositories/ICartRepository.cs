namespace GameShoppingMvcUI.Repositories
{
    public interface ICartRepository
    {
        Task<bool> AddItem(int gameId, int qty);
        Task<bool> RemoveItem(int gameId);
        Task<IEnumerable<ShoppingCart>> GetUserCart();
    }
}
