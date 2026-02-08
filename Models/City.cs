using System.ComponentModel.DataAnnotations.Schema;

namespace HouseRentals.Models
{
    public class City
    {
        public int CityId { get; set; }

        [Column(TypeName = "VarChar(50)")]
        public string Name { get; set; } = string.Empty;

        public ICollection<House> Houses { get; set; } = new List<House>();
    }
}
