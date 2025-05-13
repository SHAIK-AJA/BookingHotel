using System.ComponentModel.DataAnnotations;

namespace BookingHotel.Models.Entities
{
    public class Room
    {
        public int Id { get; set; }

        [Required]
        public string Type { get; set; }
       public int RoomNo { get; set; }
       public string? RoomName { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal PricePerNight { get; set; }

        public bool IsAvailable { get; set; }

        [Required]
        [Url]
        public string ImageUrl { get; set; }

        [Required]
        [Range(1, 5)]
        public double Rating { get; set; }

        [Required]
        public int HotelId { get; set; }

        public Hotel Hotel { get; set; }
    }
}
