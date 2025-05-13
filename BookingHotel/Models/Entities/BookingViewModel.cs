using System.ComponentModel.DataAnnotations;

namespace BookingHotel.Models.Entities
{
    public class BookingViewModel
    {
         
        public int RoomId { get; set; }

        [Required]
        [Display(Name = "Check-In Date")]
        public DateTime CheckInDate { get; set; }

        [Required]
        [Display(Name = "Check-Out Date")]
        public DateTime CheckOutDate { get; set; }
        [Required]
        [Range(1, 4, ErrorMessage = "Enter between 1 and 3 persons")]
        public int NoOfPersons { get; set; }
        public Room? Room { get; set; }
         
        public decimal TotalPrice { get; set; }
        //public string? RoomName { get; set; }
        [Required]
        public List<PersonalDetails> PersonDetails { get; set; } = new List<PersonalDetails>();
    }


}
