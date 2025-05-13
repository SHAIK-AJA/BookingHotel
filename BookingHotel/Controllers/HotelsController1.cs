using BookingHotel.Data;
using BookingHotel.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json;
//using BookingHotel.Migrations;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Security.Claims;
using Org.BouncyCastle.OpenSsl;
using System.Diagnostics;
using Org.BouncyCastle.Crypto.Parameters;
namespace BookingHotel.Controllers

{
    public class HotelsController1 : Controller
    {
        string clientId = "3NTg9F2LMpgp0YJq17bFP6QoVSHe0gUYNePWrhc0u4kuJTSl";
        string callbackUri = "http://localhost:5093/Hotels/Sample";
        string authorizeUrl = "https://staging.digitaltrusttech.com:44319/authorization";
        string tokenEndpoint = "https://staging.digitaltrusttech.com:44319/api/Authentication/token";
        string jwksUrl = "https://staging.digitaltrusttech.com:44319/api/Jwks/Jwksuri";
        string issuer = "https://staging.digitaltrusttech.com:44319";

        private readonly ApplicationDBContext _context;
      public HotelsController1(ApplicationDBContext context)
        {
                _context = context;
         }
            public async Task<IActionResult> Index()
            {
                var hotels = await _context.Hotels.Include(h => h.Rooms).ToListAsync();
                return View(hotels);
            }
        public async Task<IActionResult> Details(int id)
        {
            var hotel = await _context.Hotels
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hotel == null)
            {
                return NotFound();
            }
            return View(hotel); 
        }
      public IActionResult Create()
            {
                return View();
            }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hotel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hotel);
        }
       public IActionResult Callback(string code,string state)
        {
            ViewBag.code = code;
            return View();
        }
       // public Task async HotelsController1(string code)
       // {
            //return Ok();
        //}
       
        public static string CreateJwt(Dictionary<string, object> payload, RSA privateKey)
        {
            var securityKey = new RsaSecurityKey(privateKey);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);

            var header = new JwtHeader(credentials);
            var jwtPayload = new JwtPayload();

            foreach (var item in payload)
                jwtPayload.Add(item.Key, item.Value);

            var token = new JwtSecurityToken(header, jwtPayload);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
    }

