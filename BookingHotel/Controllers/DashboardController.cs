using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingHotel.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
  
   
            [Authorize(Roles = "Admin")]
            public IActionResult AdminDashboard()
            {
                return View();
            }

        [Authorize(Roles = "Customer")]
        public IActionResult CustomerDashboard()
            {
            var user = HttpContext.User;
            var isLoggedIn = user.Identity.IsAuthenticated;
            Console.WriteLine("User Authenticated: " + isLoggedIn);
            return View();
            }
        

    }
}
