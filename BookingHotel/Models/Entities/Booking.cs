using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using BookingHotel.Data;
namespace BookingHotel.Models.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
       // public Hotel Hotel { get; set; }
        public int RoomId { get; set; }
        public Room? Room { get; set; }
        public int NoOfPersons { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public DateTime BookingDate = DateTime.Now;
        public decimal TotalPrice { get; set; }
        

        // Add this property:
        public List<PersonalDetails> PersonDetails { get; set; } = new();
    }


}
