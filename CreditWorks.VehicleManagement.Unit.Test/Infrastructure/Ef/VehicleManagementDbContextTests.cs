using CreditWorks.VehicleManagement.Infrastructure.DataStore.Entities;
using CreditWorks.VehicleManagement.Infrastructure.Ef;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CreditWorks.VehicleManagement.Unit.Test.Infrastructure.Ef;

public sealed class VehicleManagementDbContextTests
{
    private static VehicleManagementDbContext CreateContext() => new(
        new DbContextOptionsBuilder<VehicleManagementDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=CreditWorksUnitModel;Trusted_Connection=True")
            .Options);

    [Fact]
    public void SeedData_WhenModelCreated_ThenIncludesFiveManufacturers()
    {
        using var context = CreateContext();
        var entity = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(Manufacturer))!;
        var names = entity.GetSeedData().Select(x => (string)x[nameof(Manufacturer.Name)]!).ToHashSet();
        Assert.True(names.SetEquals(["Mazda", "Mercedes", "Honda", "Ferrari", "Toyota"]));
    }

    [Fact]
    public void SeedData_WhenModelCreated_ThenIncludesRequiredCategories()
    {
        using var context = CreateContext();
        var entity = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(VehicleCategory))!;
        var categories = entity.GetSeedData().ToDictionary(x => (string)x[nameof(VehicleCategory.Name)]!);

        Assert.Equal(3, categories.Count);
        Assert.Equal(0m, categories["Light"][nameof(VehicleCategory.MinWeightKg)]);
        Assert.Equal(500m, categories["Light"][nameof(VehicleCategory.MaxWeightKg)]);
        Assert.Equal(500m, categories["Medium"][nameof(VehicleCategory.MinWeightKg)]);
        Assert.Equal(2500m, categories["Medium"][nameof(VehicleCategory.MaxWeightKg)]);
        Assert.Equal(2500m, categories["Heavy"][nameof(VehicleCategory.MinWeightKg)]);
        Assert.Null(categories["Heavy"][nameof(VehicleCategory.MaxWeightKg)]);
        Assert.All(categories.Values, x => Assert.False(string.IsNullOrWhiteSpace((string)x[nameof(VehicleCategory.IconKey)]!)));
    }

    [Fact]
    public void VehicleManagementDbContext_WhenModelCreated_ThenUsesTwoDecimalWeightColumns()
    {
        using var context = CreateContext();
        var vehicle = context.Model.FindEntityType(typeof(Vehicle))!;
        var category = context.Model.FindEntityType(typeof(VehicleCategory))!;
        Assert.Equal(2, vehicle.FindProperty(nameof(Vehicle.WeightKg))!.GetScale());
        Assert.Equal(2, category.FindProperty(nameof(VehicleCategory.MinWeightKg))!.GetScale());
        Assert.Equal(2, category.FindProperty(nameof(VehicleCategory.MaxWeightKg))!.GetScale());
    }

    [Fact]
    public void VehicleManagementDbContext_WhenModelCreated_ThenVehicleIdentifiersAreUnique()
    {
        using var context = CreateContext();
        var vehicle = context.Model.FindEntityType(typeof(Vehicle))!;

        Assert.True(vehicle.GetIndexes().Single(index =>
            index.Properties.SingleOrDefault()?.Name == nameof(Vehicle.Vin)).IsUnique);
        Assert.True(vehicle.GetIndexes().Single(index =>
            index.Properties.SingleOrDefault()?.Name == nameof(Vehicle.RegistrationNumber)).IsUnique);
    }
}
