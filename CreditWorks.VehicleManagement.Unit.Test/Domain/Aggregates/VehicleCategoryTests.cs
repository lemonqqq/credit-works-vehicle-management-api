using CreditWorks.VehicleManagement.Domain.Exceptions;
using Category = CreditWorks.VehicleManagement.Domain.Aggregates.VehicleCategory;

namespace CreditWorks.VehicleManagement.Unit.Test.Domain.Aggregates;

public sealed class VehicleCategoryTests
{
    [Fact]
    public void Create_WhenCategoryValid_ThenKeepsIconAndOpenUpperBound()
    {
        var category = Category.Create("Heavy", "Heavy vehicles", 2500m, null, "truck");
        Assert.NotEqual(Guid.Empty, category.Id);
        Assert.Equal("truck", category.IconKey);
        Assert.Null(category.MaxWeightKg);
    }

    [Theory]
    [InlineData("", "truck", "0", "500")]
    [InlineData("   ", "truck", "0", "500")]
    [InlineData("Light", "", "0", "500")]
    [InlineData("Light", "truck", "-0.01", "500")]
    [InlineData("Light", "truck", "500", "500")]
    [InlineData("Light", "truck", "600", "500")]
    [InlineData("Light", "truck", "0.001", "500")]
    public void Create_WhenCategoryDetailsInvalid_ThenRejectsCategory(string name, string icon, string min, string max)
    {
        var lower = decimal.Parse(min, System.Globalization.CultureInfo.InvariantCulture);
        var upper = decimal.Parse(max, System.Globalization.CultureInfo.InvariantCulture);
        Assert.Throws<BusinessValidationException>(() => Category.Create(name, "Description", lower, upper, icon));
    }

    [Fact]
    public void Rehydrate_WhenCategoryExists_ThenKeepsIdentifier()
    {
        var id = Guid.NewGuid();
        var category = Category.Rehydrate(id, "Light", "Light vehicles", 0m, 500m, "car");
        Assert.Equal(id, category.Id);
    }
}
