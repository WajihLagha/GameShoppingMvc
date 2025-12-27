using Microsoft.AspNetCore.Mvc;

namespace GameShoppingMvcUI.Controllers
{
    public class CartController : Controller
    {
        public readonly ICartRepository _cartRepo;
        public readonly IReservationRepository _reservationRepo;
        public readonly IPackRepository _packRepo;

        public CartController(ICartRepository cartRepo, IReservationRepository reservationRepo, IPackRepository packRepo) 
        {
            _cartRepo = cartRepo;
            _reservationRepo = reservationRepo;
            _packRepo = packRepo;
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
        public async Task<IActionResult> Checkout()
        {
            var cart = await _cartRepo.GetUserCart();
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(string fullName, string email, string phone, string address, 
            string? reservationDate, string? reservationStartTime, string? reservationEndTime, int? reservationGameId)
        {
            // Check for reservation conflict if reservation data is provided
            if (!string.IsNullOrEmpty(reservationDate) && !string.IsNullOrEmpty(reservationStartTime) 
                && !string.IsNullOrEmpty(reservationEndTime) && reservationGameId.HasValue)
            {
                try
                {
                    var date = DateTime.Parse(reservationDate);
                    var startTime = TimeSpan.Parse(reservationStartTime);
                    var endTime = TimeSpan.Parse(reservationEndTime);

                    // Check for conflicts
                    var hasConflict = await _reservationRepo.CheckConflict(reservationGameId.Value, date, startTime, endTime);
                    if (hasConflict)
                    {
                        TempData["ErrorMessage"] = "Reservation conflict! This time slot is already booked. Please select a different time.";
                        return RedirectToAction(nameof(Checkout));
                    }

                    // Create reservation
                    var reservation = await _reservationRepo.CreateReservation(reservationGameId.Value, date, startTime, endTime);
                    if (reservation != null)
                    {
                        TempData["ReservationId"] = reservation.Id;
                        TempData["ReservationDate"] = date.ToString("MMM dd, yyyy");
                        TempData["ReservationTime"] = $"{startTime:hh\\:mm} - {endTime:hh\\:mm}";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error processing reservation: {ex.Message}";
                    return RedirectToAction(nameof(Checkout));
                }
            }

            // Process the order
            bool isCheckout = await _cartRepo.DoCheckout();
            if (!isCheckout)
            {
                TempData["ErrorMessage"] = "Checkout failed. Please try again.";
                return RedirectToAction(nameof(Checkout));
            }

            // Pass the customer details to the success page
            TempData["CustomerName"] = fullName;
            TempData["OrderEmail"] = email;
            return RedirectToAction("OrderSuccess");
        }

        public IActionResult OrderSuccess()
        {
            ViewBag.CustomerName = TempData["CustomerName"] ?? "Customer";
            ViewBag.OrderEmail = TempData["OrderEmail"] ?? "";
            ViewBag.ReservationDate = TempData["ReservationDate"];
            ViewBag.ReservationTime = TempData["ReservationTime"];
            return View();
        }
    }
}
