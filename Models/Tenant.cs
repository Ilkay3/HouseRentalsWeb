using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HouseRentals.Models
{
    public class Tenant
    {
        [Key]
        [Column("tenant_id")]
        public int TenantId { get; set; }

        [Column("first_name")]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Column("last_name")]
        [MaxLength(50)]
        public string LastName { get; set; } = null!;

        [Column("phone_number")]
        [MaxLength(50)]
        public string PhoneNumber { get; set; } = null!;

        [Column("email")]
        [MaxLength(50)]
        public string Email { get; set; } = null!;

        [Column("EGN")]
        [MaxLength(50)]
        public string EGN { get; set; } = null!;
    }
}
