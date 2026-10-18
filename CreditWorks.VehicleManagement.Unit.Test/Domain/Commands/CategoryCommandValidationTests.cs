using System.ComponentModel.DataAnnotations;
using CreditWorks.VehicleManagement.Domain.Commands;

namespace CreditWorks.VehicleManagement.Unit.Test.Domain.Commands;

public sealed class CategoryCommandValidationTests
{
    private static CategoryDto ValidCategory() => new()
    {
        Name = "Medium", Description = "Medium vehicles", MinWeightKg = 500m,
        MaxWeightKg = 2500m, IconKey = "van"
    };

    // DataAnnotations does not recursively validate CategoryCommand.Categories.
    private static IReadOnlyList<ValidationResult> Validate(CategoryDto category)
    {
        var errors = new List<ValidationResult>();
        Validator.TryValidateObject(category, new ValidationContext(category), errors, true);
        return errors;
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CategoryDto_WhenNameBlank_ThenValidationFails(string name)
    {
        var category = ValidCategory();
        category.Name = name;
        Assert.Contains(Validate(category), error => error.MemberNames.Contains(nameof(CategoryDto.Name)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CategoryDto_WhenIconBlank_ThenValidationFails(string icon)
    {
        var category = ValidCategory();
        category.IconKey = icon;
        Assert.Contains(Validate(category), error => error.MemberNames.Contains(nameof(CategoryDto.IconKey)));
    }

    [Fact]
    public void CategoryDto_WhenLowerBoundNegative_ThenValidationFails()
    {
        var category = ValidCategory();
        category.MinWeightKg = -0.01m;
        Assert.NotEmpty(Validate(category));
    }

    [Fact]
    public void CategoryDto_WhenValid_ThenHasNoPropertyErrors() => Assert.Empty(Validate(ValidCategory()));
}
