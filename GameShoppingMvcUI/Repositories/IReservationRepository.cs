namespace GameShoppingMvcUI.Repositories
{
    public interface IReservationRepository
    {
        Task<bool> CheckConflict(int gameId, DateTime date, TimeSpan startTime, TimeSpan endTime, string? userId = null);
        Task<Reservation?> CreateReservation(int gameId, DateTime date, TimeSpan startTime, TimeSpan endTime);
        Task<List<Reservation>> GetUserReservations(string userId);
        Task<bool> CancelReservation(int reservationId);
        Task<Reservation?> GetReservationById(int id);
    }
}
