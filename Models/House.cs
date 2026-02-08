using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseRentals.Models
{
    public class House
    {
        public int HouseId { get; set; }

        [Column(TypeName = "VarChar(50)")]
        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "VarChar(50)")]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "DOUBLE")]
        public double Price_Per_Month { get; set; }

        [Column("Available")]
        public bool Available { get; set; } = true;

        public int OwnerId { get; set; }
        public Owner Owner { get; set; } = null!;

    }

}
