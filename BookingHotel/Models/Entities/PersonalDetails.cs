using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using BookingHotel.Data;
namespace BookingHotel.Models.Entities
{
    public class PersonalDetails
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }
        [Required]
        public string Gender { get; set; }
      //  public int No
        // Foreign key
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }
    }

}
