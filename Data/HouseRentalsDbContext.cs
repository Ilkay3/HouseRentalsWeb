using HouseRentals.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HouseRentals.Data
{
    public class HouseRentalsDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<House> Houses { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<HouseAmenities> House_Amenities { get; set; }
        public DbSet<Tenant> Tenants { get; set; }

        public HouseRentalsDbContext(
            DbContextOptions<HouseRentalsDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HouseAmenities>()
                .HasKey(ha => new { ha.HouseId, ha.AmenityId });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(
                    "Server=localhost;Database=House_Rentals_Web;User=root;Password=21012007;",
                    new MySqlServerVersion(new Version(8, 0, 41)));
            }
        }
    }
}
