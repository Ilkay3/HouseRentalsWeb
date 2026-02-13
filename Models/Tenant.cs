using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HouseRentals.Models
{
    public class Tenant
    {
        public int TenantId { get; set; }

        [Column(TypeName = "VarChar(50)")]
        [Required]
        public string First_Name { get; set; } = string.Empty;

        [Column(TypeName = "VarChar(50)")]
        [Required]
        public string Last_Name { get; set; } = string.Empty;

        [Column(TypeName = "VarChar(10)")]
        [Required]
        public string EGN { get; set; } = string.Empty;

        [Column(TypeName = "VarChar(50)")]
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Column(TypeName = "VarChar(50)")]
        [Required]
        public string Email { get; set; } = string.Empty;

        public string ApplicationUserId { get; set; } = string.Empty;

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    }

}
