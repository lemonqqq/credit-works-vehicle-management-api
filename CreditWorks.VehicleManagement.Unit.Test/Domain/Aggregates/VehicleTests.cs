using CreditWorks.VehicleManagement.Domain.Exceptions;
using AggregateVehicle = CreditWorks.VehicleManagement.Domain.Aggregates.Vehicle;

namespace CreditWorks.VehicleManagement.Unit.Test.Domain.Aggregates;

public sealed class VehicleTests
{
    private static readonly Guid MazdaId = Guid.Parse("10000000-0000-5000-9000-000000000001");

    private static AggregateVehicle Create(string owner = "Alex Smith", int year = 2020,
        decimal weightKg = 1850.75m, string vin = "ABC12345678901234",
        string registration = "ABC123") =>
        AggregateVehicle.Create(owner, MazdaId, year, vin, registration, weightKg);

    [Fact]
    public void Create_WhenVehicleValid_ThenPreservesExactWeight()
    {
        var vehicle = Create();
        Assert.NotEqual(Guid.Empty, vehicle.Id);
        Assert.Equal(1850.75m, vehicle.WeightKg);
    }

    [Fact]
    public void Rehydrate_WhenVehicleExists_ThenKeepsIdentifier()
    {
        var id = Guid.NewGuid();
        var vehicle = AggregateVehicle.Rehydrate(id, "Alex Smith", MazdaId, "Mazda", 2020,
            "ABC12345678901234", "ABC123", 1850.75m);
        Assert.Equal(id, vehicle.Id);
    }

    [Theory]
    [InlineData("", 2020, "1850.75")]
    [InlineData("   ", 2020, "1850.75")]
    [InlineData("Alex Smith", 3000, "1850.75")]
    [InlineData("Alex Smith", 2020, "0")]
    [InlineData("Alex Smith", 2020, "-0.01")]
    [InlineData("Alex Smith", 2020, "12.345")]
    public void Create_WhenRequiredDetailsInvalid_ThenRejectsVehicle(string owner, int year, string weight)
    {
        var value = decimal.Parse(weight, System.Globalization.CultureInfo.InvariantCulture);
        Assert.Throws<BusinessValidationException>(() => Create(owner: owner, year: year, weightKg: value));
    }

    [Fact]
    public void Update_WhenWeightHasTwoDecimals_ThenPreservesPrecision()
    {
        var vehicle = Create();
        vehicle.Update("Alex Smith", MazdaId, 2020, "ABC12345678901234", "ABC123", 499.99m);
        Assert.Equal(499.99m, vehicle.WeightKg);
    }

    [Fact]
    public void Create_WhenIdentifiersMissing_ThenRejectsCurrentVehicleContract()
    {
        Assert.Throws<BusinessValidationException>(() =>
            AggregateVehicle.Create("Alex Smith", MazdaId, 2020, null!, null!, 1850.75m));
    }
}
