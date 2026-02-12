using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseRentals.Models
{
    public class House
    {
        [Key]
        public int HouseId { get; set; }

        [Column(TypeName = "VarChar(50)")]
        [Required(ErrorMessage = "Адресът е задължителен")]
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Описанието е задължително")]
        [Column(TypeName = "VarChar(500)")]
        [Display(Name = "Description")]
        [StringLength(500, ErrorMessage = "Описанието не може да надвишава 500 символа")]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "DOUBLE")]
        [Required(ErrorMessage = "Цената е задължителна")]
        [Range(0, 100000, ErrorMessage = "Цената трябва да е между 0 и 100000")]
        [Display(Name = "Price per month")]
        public double Price_Per_Month { get; set; }

        [Column("Available")]
        [Display(Name = "Available")]
        public bool Available { get; set; } = true;

        // Връзка със собственик (Owner)
        [Required]
        public int OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public Owner? Owner { get; set; }

        // Връзка с наемател (Tenant) - nullable, защото може да няма наемател
        public int? TenantId { get; set; }

        [ForeignKey("TenantId")]
        public Tenant? Tenant { get; set; }

        // Връзка с град (City) - нова!
        public int? CityId { get; set; }

        [ForeignKey("CityId")]
        [Display(Name = "City")]
        public City? City { get; set; }

        // Дата на създаване
        [Column(TypeName = "DATETIME")]
        [Display(Name = "Created on")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        // Дата на последна промяна
        [Column(TypeName = "DATETIME")]
        [Display(Name = "Last updated")]
        public DateTime? UpdatedAt { get; set; }

        // Навигационно пропърти за удобства (Amenities)
        public ICollection<HouseAmenities> HouseAmenities { get; set; } = new List<HouseAmenities>();

        // ✅ Изчисляемо пропърти - пълния адрес (ако има град)
        [NotMapped]
        public string FullAddress => City != null
            ? $"{Address}, {City.Name}"
            : Address;
    }
}