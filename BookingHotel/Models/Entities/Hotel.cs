using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace BookingHotel.Models.Entities
{
    public class Hotel
    {
        public int Id { get; set; }


        public required string Name { get; set; } = string.Empty;


        public required string Location { get; set; } = string.Empty;


        public required string Description { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public List<Room> Rooms { get; set; } = new();
    }
}
