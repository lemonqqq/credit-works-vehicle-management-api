using CreditWorks.VehicleManagement.Infrastructure.DataStore.Entities;
using Microsoft.EntityFrameworkCore;

namespace CreditWorks.VehicleManagement.Infrastructure.Ef.Seed
{
    /// <summary>
    ///     Defines the initial manufacturers, vehicles, and weight categories.
    /// </summary>
    public static class SeedData
    {
        /// <summary>
        ///     Adds the initial data to the Entity Framework model.
        /// </summary>
        /// <param name="builder">The model builder receiving the seed data.</param>
        public static void Seed(ModelBuilder builder)
        {
            SeedManufacturers(builder);
            SeedVehicles(builder);
            SeedVehicleCategories(builder);
        }

        /// <summary>
        ///     Adds the predefined manufacturers.
        /// </summary>
        /// <param name="builder">The model builder receiving the seed data.</param>
        private static void SeedManufacturers(ModelBuilder builder)
        {
            var manufacturers = new List<Manufacturer>
            {
                new(Guid.Parse("10000000-0000-5000-9000-000000000001"), "Mazda"),
                new(Guid.Parse("10000000-0000-5000-9000-000000000002"), "Mercedes"),
                new(Guid.Parse("10000000-0000-5000-9000-000000000003"), "Honda"),
                new(Guid.Parse("10000000-0000-5000-9000-000000000004"), "Ferrari"),
                new(Guid.Parse("10000000-0000-5000-9000-000000000005"), "Toyota")
            };
            builder.Entity<Manufacturer>().HasData(manufacturers);
        }

        /// <summary>
        ///     Adds sample vehicles for the initial database.
        /// </summary>
        /// <param name="builder">The model builder receiving the seed data.</param>
        private static void SeedVehicles(ModelBuilder builder)
        {
            var vehicles = new List<Vehicle>
            {
                new(Guid.Parse("10000000-0000-4000-8000-000000000001"), "Amelia Wilson", Guid.Parse("10000000-0000-5000-9000-000000000001"), 2018, "CWMZD000000000001", "CW0001", 420.50m),
                new(Guid.Parse("10000000-0000-4000-8000-000000000002"), "Noah Thompson", Guid.Parse("10000000-0000-5000-9000-000000000001"), 2020, "CWMZD000000000002", "CW0002", 499.99m),
                new(Guid.Parse("10000000-0000-4000-8000-000000000003"), "Isla Martin", Guid.Parse("10000000-0000-5000-9000-000000000001"), 2022, "CWMZD000000000003", "CW0003", 500.00m),
                new(Guid.Parse("10000000-0000-4000-8000-000000000004"), "Oliver Chen", Guid.Parse("10000000-0000-5000-9000-000000000001"), 2017, "CWMZD000000000004", "CW0004", 725.25m),
                new(Guid.Parse("10000000-0000-4000-8000-000000000005"), "Sophie Patel", Guid.Parse("10000000-0000-5000-9000-000000000002"), 2019, "CWMRC000000000005", "CW0005", 1850.75m),
                new(Guid.Parse("10000000-0000-4000-8000-000000000006"), "Liam Roberts", Guid.Parse("10000000-0000-5000-9000-000000000002"), 2021, "CWMRC000000000006", "CW0006", 2200.00m),
                new(Guid.Parse("10000000-0000-4000-8000-000000000007"), "Mia Anderson", Guid.Parse("10000000-0000-5000-9000-000000000002"), 2016, "CWMRC000000000007", "CW0007", 2499.99m),
                new(Guid.Parse("10000000-0000-4000-8000-000000000008"), "Ethan Walker", Guid.Parse("10000000-0000-5000-9000-000000000002"), 2023, "CWMRC000000000008", "CW0008", 2500.00m),
                new(Guid.Parse("10000000-0000-4000-8000-000000000009"), "Ava Brown", Guid.Parse("10000000-0000-5000-9000-000000000003"), 2015, "CWHND000000000009", "CW0009", 1500.00m),
                new(Guid.Parse("10000000-0000-4000-8000-000000000010"), "Lucas Taylor", Guid.Parse("10000000-0000-5000-9000-000000000003"), 2024, "CWHND000000000010", "CW0010", 500.00m),
                new(Guid.Parse("10000000-0000-4000-8000-000000000011"), "Grace Williams", Guid.Parse("10000000-0000-5000-9000-000000000003"), 2014, "CWHND000000000011", "CW0011", 499.50m),
                new(Guid.Parse("10000000-0000-4000-8000-000000000012"), "Jack Harris", Guid.Parse("10000000-0000-5000-9000-000000000003"), 2020, "CWHND000000000012", "CW0012", 980.10m),
                new(Guid.Parse("10000000-0000-4000-8000-000000000013"), "Charlotte King", Guid.Parse("10000000-0000-5000-9000-000000000004"), 2018, "CWFRR000000000013", "CW0013", 1350.00m),
                new(Guid.Parse("10000000-0000-4000-8000-000000000014"), "James Cooper", Guid.Parse("10000000-0000-5000-9000-000000000004"), 2022, "CWFRR000000000014", "CW0014", 1600.25m),
                new(Guid.Parse("10000000-0000-4000-8000-000000000015"), "Ella Mitchell", Guid.Parse("10000000-0000-5000-9000-000000000004"), 2017, "CWFRR000000000015", "CW0015", 2050.00m),
                new(Guid.Parse("10000000-0000-4000-8000-000000000016"), "Benjamin Scott", Guid.Parse("10000000-0000-5000-9000-000000000004"), 2021, "CWFRR000000000016", "CW0016", 2600.00m),
                new(Guid.Parse("10000000-0000-4000-8000-000000000017"), "Harper Young", Guid.Parse("10000000-0000-5000-9000-000000000005"), 2019, "CWTYT000000000017", "CW0017", 2500.01m),
                new(Guid.Parse("10000000-0000-4000-8000-000000000018"), "Henry Adams", Guid.Parse("10000000-0000-5000-9000-000000000005"), 2023, "CWTYT000000000018", "CW0018", 3200.00m),
                new(Guid.Parse("10000000-0000-4000-8000-000000000019"), "Ruby Campbell", Guid.Parse("10000000-0000-5000-9000-000000000005"), 2016, "CWTYT000000000019", "CW0019", 499.99m),
                new(Guid.Parse("10000000-0000-4000-8000-000000000020"), "Daniel Morgan", Guid.Parse("10000000-0000-5000-9000-000000000005"), 2025, "CWTYT000000000020", "CW0020", 5800.00m)
            };

            builder.Entity<Vehicle>().HasData(vehicles);
        }

        /// <summary>
        ///     Adds the initial Light, Medium, and Heavy categories.
        /// </summary>
        /// <param name="builder">The model builder receiving the seed data.</param>
        private static void SeedVehicleCategories(ModelBuilder builder)
        {
            var categories = new List<VehicleCategory>
            {
                new(Guid.Parse("10000000-0000-3000-7000-000000000001"), "Light", "Light - up to 500 kg", 0.00m, 500.00m, "car"),
                new(Guid.Parse("10000000-0000-3000-7000-000000000002"), "Medium", "Medium - 500 kg to 2500 kg", 500.00m, 2500.00m, "van"),
                new(Guid.Parse("10000000-0000-3000-7000-000000000003"), "Heavy", "Heavy - 2500 kg and above", 2500.00m, null, "truck")
            };
            builder.Entity<VehicleCategory>().HasData(categories);
        }
    }
}
