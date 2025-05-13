using BookingHotel.Data;
using BookingHotel.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BookingHotel.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace BookingHotel.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
       // private readonly IEmailService _emailService;
        public BookingsController( ApplicationDBContext context, UserManager<ApplicationUser> userManager)
        {
           // _emailService = emailService;
            _context = context;
            _userManager = userManager;
        }

        // GET: Bookings/Create
        
        // GET: Bookings/Create
      //  public async Task<IActionResult> Create(int roomId)
       public async Task<IActionResult> Create(int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null || !room.IsAvailable)
                return NotFound();

            DateTime checkIn = DateTime.Now;
            DateTime checkOut = DateTime.Now.AddDays(1);
            int nights = (checkOut - checkIn).Days;

            var model = new BookingViewModel
            {
                RoomId = roomId,
                Room = room,
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                TotalPrice = nights * room.PricePerNight,
                PersonDetails = new List<PersonalDetails> { new PersonalDetails() }
            };

            return View(model);
        }



        // POST: Bookings/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingViewModel model)
        {
            var room = await _context.Rooms.FindAsync(model.RoomId);
            model.Room = room;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var nights = (model.CheckOutDate - model.CheckInDate).Days;
            model.TotalPrice = nights * room.PricePerNight;

            // 🔑 Get current user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var booking = new Booking
            {
                RoomId = model.RoomId,
                CheckInDate = model.CheckInDate,
                CheckOutDate = model.CheckOutDate,
                NoOfPersons = model.NoOfPersons,
                PersonDetails = model.PersonDetails,
                UserId = userId   // 🔥 Assign the user
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            // ❌ Update room availability
            room.IsAvailable = false;
            
            return RedirectToAction("MyBookings");
        }

        // GET: Bookings/MyBookings
        public async Task<IActionResult> MyBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .Include(b => b.PersonDetails)
                .Where(b => b.UserId == user.Id)
                .ToListAsync();

            return View(bookings);
        }
         
        // GET: Bookings/All (Admin only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> All()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .Include(b => b.User)
                .Include(b => b.PersonDetails)
                .ToListAsync();

            return View(bookings);
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.PersonDetails)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            return View(booking);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .Include(b => b.PersonDetails)
                .Include(b => b.User)
                .ToListAsync();

            return View(bookings);
        }

    }
}
