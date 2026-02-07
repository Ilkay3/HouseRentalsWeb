using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HouseRentals.Models
{
    public class Amenity
    {
        [Key]
        [Column("amenity_id")]
        public int AmenityId { get; set; }

        [Column("name")]
        public string Name { get; set; } = null!;

        public ICollection<HouseAmenities> HouseAmenities { get; set; }
            = new List<HouseAmenities>();
    }
}
