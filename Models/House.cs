using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseRentals.Models
{
    public class House
    {
        [Key]
        [Column("house_id")]
        public int HouseId { get; set; }

        [Column("address")]
        public string Address { get; set; } = null!;

        [Column("price_per_month")]
        public int PricePerMonth { get; set; }

        [Column("owner_id")]
        public int OwnerId { get; set; }

        public Owner Owner { get; set; } = null!;

        public ICollection<HouseAmenities> HouseAmenities { get; set; }
            = new List<HouseAmenities>();
    }

}
