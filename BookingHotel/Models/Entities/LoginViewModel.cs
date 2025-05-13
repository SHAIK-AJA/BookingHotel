 
using System.ComponentModel.DataAnnotations;
namespace BookingHotel.Models.Entities
{
    public class LoginViewModel
    {
        //[Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        //[Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
        // ✅ Add this property:
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
