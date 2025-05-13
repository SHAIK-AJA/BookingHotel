using BookingHotel.Data;
using BookingHotel.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
//using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Security.Claims;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto.Parameters;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Headers;
using Azure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.Text;
using Newtonsoft.Json;

namespace BookingHotel.Controllers
{
    public class HotelsController : Controller
    {
        private readonly ApplicationDBContext _context;

        private readonly string clientId = "3NTg9F2LMpgp0YJq17bFP6QoVSHe0gUYNePWrhc0u4kuJTSl";
        private readonly string callbackUri = "http://localhost:5093/Hotels/Callback";
        private readonly string authorizeUrl = "https://staging.digitaltrusttech.com:44319/authorization";
        private readonly string tokenEndpoint = "https://staging.digitaltrusttech.com:44319/api/Authentication/token";
        private readonly string issuer = "https://staging.digitaltrusttech.com:44319";
        private readonly string UserInfoEndpoint = "https://staging.digitaltrusttech.com:44319/api/UserInfo/userinfo";
        private readonly string clientsecret = "KXYNAy2UY2hf1ESmMo3psRDnWAMDF53oEpLu5Ua5cmgSjsUABsKyojW8k83O9y2C";
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly ILogger<HotelsController> _logger;
        private readonly HttpClient _httpClient;

        public HotelsController( ApplicationDBContext context,UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            //logger = logger;
            //_httpClient = httpClient;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
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
        public IActionResult Authenticate()
        {
            var privateKey = LoadPrivateKey("privatekey.pem");

            var now = DateTime.UtcNow;
            var payload = new Dictionary<string, object>
            {
                { "iat", new DateTimeOffset(now).ToUnixTimeSeconds() },
                { "iss", clientId },
                { "sub", clientId },
                { "aud", issuer },
                { "exp", new DateTimeOffset(now.AddMinutes(5)).ToUnixTimeSeconds() },
                { "redirect_uri", callbackUri }
            };

            var clientJwt = CreateJwt(payload, privateKey);

            var authUrl = $"{authorizeUrl}?client_id={clientId}" +
                          $"&redirect_uri={Uri.EscapeDataString(callbackUri)}" +
                          $"&response_type=code" +
                          $"&scope=openid urn:idp:digitalid:profile" +
                          $"&request={clientJwt}" +
                          $"&state=asdauysdiasdad8as87da8sd69asd5as9d6" +
                          $"&nonce=asdauysdiasdad8as87da8sd69asd5as9d6";
           // Console.WriteLine("Redirecting to Auth URL:");
            //Console.WriteLine(authUrl);

            return Redirect(authUrl);

        }

        public async Task<IActionResult> Callback(string code, string state)
        {
            string codecpy = code;
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Authorization code is missing.");
            }

            var privateKey = LoadPrivateKey("privatekey.pem");
            var now = DateTime.UtcNow;

            var clientAssertionPayload = new Dictionary<string, object>
    {
        { "iss", clientId },
        { "sub", clientId },
        { "aud", tokenEndpoint }, 
      //  { "jti", Guid.NewGuid().ToString() },
        { "iat", new DateTimeOffset(now).ToUnixTimeSeconds() },
        { "exp", new DateTimeOffset(now.AddMinutes(5)).ToUnixTimeSeconds() }
    };

            var clientAssertion = CreateJwt(clientAssertionPayload, privateKey);
            //Console.WriteLine("Client Assertion is   n..................");
            //Console.WriteLine(clientAssertion);
            var postData = new Dictionary<string, string>
    {
        { "grant_type", "authorization_code" },
        { "code", codecpy },
        { "redirect_uri", callbackUri },
        { "client_id", clientId },
        { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
        { "client_assertion", clientAssertion }
    };

            using var httpClient = new HttpClient();
            var content = new FormUrlEncodedContent(postData);
          //  Console.WriteLine("   url encided iss     ");
           // Console.WriteLine(content);
            var response = await httpClient.PostAsync(tokenEndpoint, content);
            var responseString = await response.Content.ReadAsStringAsync();
          //  Console.WriteLine("Response:");
            //Console.WriteLine(responseString);
            if (!response.IsSuccessStatusCode)
            {
                return Content($"Token exchange failed: {response.StatusCode} \n\n{responseString}");
            }
            var tokenData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseString);
        //    Console.WriteLine(JsonConvert.SerializeObject(tokenData, Formatting.Indented)); // ✅ Debug print

            if (!tokenData.ContainsKey("access_token"))
            {
                return BadRequest("Access token was not returned. Response: " + responseString);
            }
            var accessToken = tokenData["access_token"].ToString();
            var jwttoken = tokenData["id_token"].ToString();
            //Console.WriteLine("ACCESSS TOKEN   IS");
            //Console.WriteLine(accessToken);
            HttpContext.Session.SetString("access_token", accessToken);

            var userInfoRequest = new HttpRequestMessage(HttpMethod.Get, UserInfoEndpoint);
            userInfoRequest.Headers.Add("UgPassAuthorization", $"Bearer {accessToken}");
            userInfoRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jwt"));
            userInfoRequest.Headers.ConnectionClose = true;

            var responseinfo = await httpClient.SendAsync(userInfoRequest, HttpCompletionOption.ResponseHeadersRead);
           // Console.WriteLine(responseinfo);
            if (!responseinfo.IsSuccessStatusCode)
            {
                var err = await responseinfo.Content.ReadAsStringAsync();
                return Content($"UserInfo call failed: {responseinfo.StatusCode}\n{err}");
            }
            var parts = jwttoken.Split('.');
            if (parts.Length != 3)
            {
                Console.WriteLine("Invalid JWT token.");
                
            }
            string payload = parts[1];
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            var bytes = Convert.FromBase64String(payload.Replace('-', '+').Replace('_', '/'));
            string jsonPayload = Encoding.UTF8.GetString(bytes);
            using var doc = JsonDocument.Parse(jsonPayload);
            if (!doc.RootElement.TryGetProperty("daes_claims", out JsonElement daesClaims) ||
         !daesClaims.TryGetProperty("email", out JsonElement emailElement))
            {
                return BadRequest("Email not found in JWT.");
            }
            string email = emailElement.GetString();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser { UserName = email, Email = email };

                bool havingRole = await _userManager.IsInRoleAsync(user, "Customer");

                var createResult = await _userManager.CreateAsync(user);
                if (!havingRole)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                }
                if (!createResult.Succeeded)
                {
                    return BadRequest("User creation failed.");
                }
            }
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("CustomerDashboard", "Dashboard");
           }
        private string CreateJwt(Dictionary<string, object> payload, AsymmetricKeyParameter privateKey)
        {
            var header = new Dictionary<string, object>
    {
        { "alg", "RS256" },
        { "typ", "JWT" }
    };

            var headerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header));
            var payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));

            string encodedHeader = Base64UrlEncode(headerBytes);
            string encodedPayload = Base64UrlEncode(payloadBytes);

            string message = $"{encodedHeader}.{encodedPayload}";
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            ISigner signer = SignerUtilities.GetSigner("SHA256withRSA");
            signer.Init(true, privateKey);
            signer.BlockUpdate(messageBytes, 0, messageBytes.Length);
            byte[] signature = signer.GenerateSignature();

            string encodedSignature = Base64UrlEncode(signature);
            return $"{message}.{encodedSignature}";
        }

        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private AsymmetricKeyParameter LoadPrivateKey(string privateKeyPath)
        {
            using var reader = System.IO.File.OpenText(privateKeyPath);
            var pemReader = new PemReader(reader);
            return (AsymmetricKeyParameter)pemReader.ReadObject();
        }

    }
}
