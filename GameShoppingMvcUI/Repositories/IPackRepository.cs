namespace GameShoppingMvcUI.Repositories
{
    public interface IPackRepository
    {
        Task<IEnumerable<Pack>> GetAllPacks();
        Task<Pack?> GetPackById(int id);
        Task<bool> AddPack(Pack pack, List<int> gameIds);
        Task<bool> UpdatePack(Pack pack, List<int> gameIds);
        Task<bool> DeletePack(int id);
        Task<List<Pack>> GetActivePacksForGames(List<int> gameIds);
    }
}
