using System.ComponentModel.DataAnnotations;
using CreditWorks.VehicleManagement.Domain.Commands;

namespace CreditWorks.VehicleManagement.Unit.Test.Domain.Commands;

public sealed class VehicleCommandValidationTests
{
    private static VehicleCommand ValidCommand() => new()
    {
        OwnerName = "Alex Smith",
        Manufacturer = "Mazda",
        Year = 2020,
        WeightKg = 1850.75m,
        Vin = "ABC12345678901234",
        RegistrationNumber = "ABC123"
    };

    private static IReadOnlyList<ValidationResult> Validate(VehicleCommand command)
    {
        var errors = new List<ValidationResult>();
        Validator.TryValidateObject(command, new ValidationContext(command), errors, validateAllProperties: true);
        return errors;
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void VehicleCommand_WhenOwnerBlank_ThenValidationFails(string ownerName)
    {
        var command = ValidCommand();
        command.OwnerName = ownerName;
        Assert.Contains(Validate(command), error => error.MemberNames.Contains(nameof(VehicleCommand.OwnerName)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void VehicleCommand_WhenManufacturerBlank_ThenValidationFails(string manufacturer)
    {
        var command = ValidCommand();
        command.Manufacturer = manufacturer;
        Assert.Contains(Validate(command), error => error.MemberNames.Contains(nameof(VehicleCommand.Manufacturer)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1800)]
    [InlineData(3000)]
    public void VehicleCommand_WhenYearUnsensible_ThenValidationFails(int year)
    {
        var command = ValidCommand();
        command.Year = year;
        Assert.Contains(Validate(command), error => error.MemberNames.Contains(nameof(VehicleCommand.Year)));
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-0.01")]
    public void VehicleCommand_WhenWeightNonpositive_ThenValidationFails(string value)
    {
        var command = ValidCommand();
        command.WeightKg = decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        Assert.Contains(Validate(command), error => error.MemberNames.Contains(nameof(VehicleCommand.WeightKg)));
    }

    [Fact]
    public void VehicleCommand_WhenWeightHasTwoDecimals_ThenValidationPasses()
    {
        Assert.Empty(Validate(ValidCommand()));
    }

    [Fact]
    public void VehicleCommand_WhenWeightIsPointZeroOne_ThenValidationPasses()
    {
        var command = ValidCommand();
        command.WeightKg = 0.01m;
        Assert.Empty(Validate(command));
    }

    [Fact]
    public void VehicleCommand_WhenIdentifiersMissing_ThenValidationFailsForCurrentContract()
    {
        var command = ValidCommand();
        command.Vin = null!;
        command.RegistrationNumber = null!;
        var errors = Validate(command);
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(VehicleCommand.Vin)));
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(VehicleCommand.RegistrationNumber)));
    }
}
