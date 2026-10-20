using CreditWorks.VehicleManagement.Application.Models.Commands;
using CreditWorks.VehicleManagement.Application.Validators;
using CreditWorks.VehicleManagement.Domain.Exceptions;

namespace CreditWorks.VehicleManagement.Unit.Test.Application.Validators;

public sealed class VehicleCategoryValidatorTests
{
    private static List<CategoryDto> Initial() =>
    [
        new() { Name = "Light", MinWeightKg = 0m, MaxWeightKg = 500m, IconKey = "car", Description = "Light" },
        new() { Name = "Medium", MinWeightKg = 500m, MaxWeightKg = 2500m, IconKey = "van", Description = "Medium" },
        new() { Name = "Heavy", MinWeightKg = 2500m, MaxWeightKg = null, IconKey = "truck", Description = "Heavy" }
    ];

    [Fact]
    public void Validate_WhenRangesAdjacent_ThenCoversPositiveWeights() => VehicleCategoryValidator.Validate(Initial());

    [Fact]
    public void Validate_WhenSharedBoundaryMoves_ThenAcceptsConfiguration()
    {
        var categories = Initial();
        categories[1].MaxWeightKg = 2000m;
        categories[2].MinWeightKg = 2000m;
        VehicleCategoryValidator.Validate(categories);
    }

    [Fact]
    public void Validate_WhenGapExists_ThenRejectsConfiguration()
    {
        var categories = Initial();
        categories[1].MinWeightKg = 600m;
        var error = Assert.Throws<BusinessConflictException>(() => VehicleCategoryValidator.Validate(categories));
        Assert.Matches("(?i)gap|missing", error.Message);
    }

    [Fact]
    public void Validate_WhenRangesOverlap_ThenRejectsConfiguration()
    {
        var categories = Initial();
        categories[1].MinWeightKg = 400m;
        var error = Assert.Throws<BusinessConflictException>(() => VehicleCategoryValidator.Validate(categories));
        Assert.Matches("(?i)overlap", error.Message);
    }

    [Fact]
    public void Validate_WhenRequiredCategoryDeleted_ThenRejectsConfiguration()
    {
        var categories = Initial();
        categories.RemoveAt(1);
        var error = Assert.Throws<BusinessConflictException>(() => VehicleCategoryValidator.Validate(categories));
        Assert.Matches("(?i)gap|missing", error.Message);
    }

    [Fact]
    public void Validate_WhenFinalRangeBounded_ThenRejectsConfiguration()
    {
        var categories = Initial();
        categories[2].MaxWeightKg = 5000m;
        var error = Assert.Throws<BusinessConflictException>(() => VehicleCategoryValidator.Validate(categories));
        Assert.Matches("(?i)open|upper|unbounded", error.Message);
    }

    [Fact]
    public void Validate_WhenNamesDuplicate_ThenRejectsConfiguration()
    {
        var categories = Initial();
        categories[1].Name = "Light";
        var error = Assert.Throws<BusinessConflictException>(() => VehicleCategoryValidator.Validate(categories));
        Assert.Matches("(?i)duplicate|unique|same name", error.Message);
    }

    [Fact]
    public void Validate_WhenIconMissing_ThenRejectsConfiguration()
    {
        var categories = Initial();
        categories[1].IconKey = "";
        var error = Assert.Throws<BusinessConflictException>(() => VehicleCategoryValidator.Validate(categories));
        Assert.Matches("(?i)icon", error.Message);
    }

    [Fact]
    public void Validate_WhenFirstRangeStartsAboveZero_ThenRejectsConfiguration()
    {
        var categories = Initial();
        categories[0].MinWeightKg = 0.01m;
        var error = Assert.Throws<BusinessConflictException>(() => VehicleCategoryValidator.Validate(categories));
        Assert.Matches("(?i)start|zero|missing", error.Message);
    }

    [Fact]
    public void Validate_WhenUpperBoundNotGreater_ThenRejectsConfiguration()
    {
        var categories = Initial();
        categories[1].MaxWeightKg = categories[1].MinWeightKg;
        var error = Assert.Throws<BusinessConflictException>(() => VehicleCategoryValidator.Validate(categories));
        Assert.Matches("(?i)bound|greater|equal|range", error.Message);
    }

    [Fact]
    public void Validate_WhenBoundaryOverPrecise_ThenRejectsConfiguration()
    {
        var categories = Initial();
        categories[1].MinWeightKg = 500.001m;
        var error = Assert.Throws<BusinessConflictException>(() => VehicleCategoryValidator.Validate(categories));
        Assert.Matches("(?i)decimal|precision|two places", error.Message);
    }
}
