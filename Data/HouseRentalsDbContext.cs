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
        public DbSet<Rental> Rentals { get; set; }

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
            
            // Rental -> House (Many-to-One)
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.House)
                .WithMany(h => h.Rentals)
                .HasForeignKey(r => r.HouseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Rental -> Tenant (Many-to-One)
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Tenant)
                .WithMany(t => t.Rentals)
                .HasForeignKey(r => r.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Индекси за бързо търсене
            modelBuilder.Entity<Rental>()
                .HasIndex(r => r.IsActive)
                .HasDatabaseName("IX_Rental_IsActive");

            modelBuilder.Entity<Rental>()
                .HasIndex(r => new { r.HouseId, r.IsActive })
                .HasDatabaseName("IX_Rental_House_Active");
        }
    }
}