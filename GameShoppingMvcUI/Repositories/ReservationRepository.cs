using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameShoppingMvcUI.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public ReservationRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<bool> CheckConflict(int gameId, DateTime date, TimeSpan startTime, TimeSpan endTime, string? userId = null)
        {
            try
            {
                // Check if there's any existing reservation for the same game on the same date
                // where the time slots overlap
                var conflict = await _db.Reservations
                    .Where(r => r.GameId == gameId 
                        && r.ReservationDate.Date == date.Date
                        && r.Status != "Cancelled"
                        && (userId == null || r.UserId != userId) // Exclude current user's reservations
                        && (
                            // Check if time slots overlap
                            (r.StartTime < endTime && r.EndTime > startTime)
                        ))
                    .AnyAsync();

                return conflict;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking reservation conflict: {ex.Message}");
                return false;
            }
        }

        public async Task<Reservation?> CreateReservation(int gameId, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User not logged in");

                // Check for conflicts before creating
                var hasConflict = await CheckConflict(gameId, date, startTime, endTime, userId);
                if (hasConflict)
                {
                    return null; // Conflict exists
                }

                var reservation = new Reservation
                {
                    UserId = userId,
                    GameId = gameId,
                    ReservationDate = date.Date,
                    StartTime = startTime,
                    EndTime = endTime,
                    Status = "Pending",
                    CreatedDate = DateTime.UtcNow
                };

                _db.Reservations.Add(reservation);
                await _db.SaveChangesAsync();

                return reservation;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating reservation: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Reservation>> GetUserReservations(string userId)
        {
            try
            {
                return await _db.Reservations
                    .Where(r => r.UserId == userId)
                    .Include(r => r.Game)
                    .OrderByDescending(r => r.ReservationDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user reservations: {ex.Message}");
                return new List<Reservation>();
            }
        }

        public async Task<bool> CancelReservation(int reservationId)
        {
            try
            {
                var reservation = await _db.Reservations.FindAsync(reservationId);
                if (reservation == null)
                    return false;

                reservation.Status = "Cancelled";
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cancelling reservation: {ex.Message}");
                return false;
            }
        }

        public async Task<Reservation?> GetReservationById(int id)
        {
            try
            {
                return await _db.Reservations
                    .Include(r => r.Game)
                    .FirstOrDefaultAsync(r => r.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting reservation: {ex.Message}");
                return null;
            }
        }

        private string? GetUserId()
        {
            var principal = _httpContextAccessor?.HttpContext?.User;
            string? userId = _userManager.GetUserId(principal);
            return userId;
        }
    }
}
