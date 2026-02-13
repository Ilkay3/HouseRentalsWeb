using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseRentals.Models
{
    public class Rental
    {
        [Key]
        public int RentalId { get; set; }

        public int HouseId { get; set; }
        [ForeignKey("HouseId")]
        public House? House { get; set; }

        public int TenantId { get; set; }
        [ForeignKey("TenantId")]
        public Tenant? Tenant { get; set; }

        [Column(TypeName = "DATETIME")]
        [Display(Name = "Наета на")]
        public DateTime RentDate { get; set; } = DateTime.Now;

        [Column(TypeName = "DATETIME")]
        [Display(Name = "Освободена на")]
        public DateTime? ReleaseDate { get; set; }

        [Column(TypeName = "DOUBLE")]
        [Display(Name = "Цена при наемане")]
        public double PriceAtRent { get; set; }

        public bool IsActive { get; set; } = true;

        [Column(TypeName = "VarChar(500)")]
        [Display(Name = "Бележки")]
        public string? Notes { get; set; }

        [Column(TypeName = "DOUBLE")]
        public double? TotalAmount { get; set; }

        public bool IsPaid { get; set; } = false;
    }
}