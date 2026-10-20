using CreditWorks.VehicleManagement.Infrastructure.DataStore.Entities;
using CreditWorks.VehicleManagement.Infrastructure.Ef.Seed;
using Microsoft.EntityFrameworkCore;

namespace CreditWorks.VehicleManagement.Infrastructure.Ef
{
    public sealed class VehicleManagementDbContext : DbContext
    {
        public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<VehicleCategory> VehicleCategories => Set<VehicleCategory>();

        public VehicleManagementDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureManufacturer(modelBuilder);
            ConfigureVehicle(modelBuilder);
            ConfigureVehicleCategory(modelBuilder);

            SeedData.Seed(modelBuilder);
        }

        private static void ConfigureManufacturer(ModelBuilder builder)
        {
            builder.Entity<Manufacturer>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name).HasMaxLength(100);
                entity.HasIndex(x => x.Name).IsUnique();
            });
        }

        private static void ConfigureVehicle(ModelBuilder builder)
        {
            builder.Entity<Vehicle>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.OwnerName).HasMaxLength(250)
                    .IsRequired();

                entity.HasIndex(x => x.ManufacturerId);

                entity.Property(x => x.RegistrationNumber).HasMaxLength(6)
                    .IsRequired();
                entity.HasIndex(x => x.RegistrationNumber)
                    .IsUnique();

                entity.Property(x => x.Year).IsRequired();

                entity.Property(x => x.Vin).HasMaxLength(17).IsFixedLength();
                entity.HasIndex(x => x.Vin)
                    .IsUnique();

                entity.Property(x => x.WeightKg)
                    .HasPrecision(18, 2)
                    .IsRequired();
            });
        }

        private static void ConfigureVehicleCategory(ModelBuilder builder)
        {
            builder.Entity<VehicleCategory>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name).HasMaxLength(100)
                    .IsRequired();
                entity.Property(x => x.Description);

                entity.Property(x => x.MinWeightKg)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(x => x.MaxWeightKg)
                    .HasPrecision(18, 2);

                entity.Property(x => x.IconKey)
                    .HasMaxLength(20)
                    .IsRequired();
            });
        }
    }
}
