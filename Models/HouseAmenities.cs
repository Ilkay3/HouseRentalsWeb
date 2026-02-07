using System.ComponentModel.DataAnnotations.Schema;

namespace HouseRentals.Models
{
    public class HouseAmenities
    {
        [Column("house_id")]
        public int HouseId { get; set; }

        public House House { get; set; } = null!;

        [Column("amenity_id")]
        public int AmenityId { get; set; }

        public Amenity Amenity { get; set; } = null!;
    }
}
