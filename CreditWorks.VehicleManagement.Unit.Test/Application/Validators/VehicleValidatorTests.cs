using CreditWorks.VehicleManagement.Application.Validators;
using CreditWorks.VehicleManagement.Domain.Aggregates;
using CreditWorks.VehicleManagement.Domain.Exceptions;

namespace CreditWorks.VehicleManagement.Unit.Test.Application.Validators;

public sealed class VehicleValidatorTests
{
    private static Vehicle VehicleWith(Guid id, string vin = "ABC12345678901234", string registration = "ABC123") =>
        new(id, "Alex Smith", Guid.NewGuid(), 2020, vin, registration, 1850.75m);

    [Fact]
    public void ValidateVin_WhenAnotherVehicleUsesIt_ThenRejectsDuplicate()
    {
        var proposed = VehicleWith(Guid.NewGuid());
        Assert.Throws<BusinessConflictException>(() => VehicleValidator.ValidateVin(
            proposed, [VehicleWith(Guid.NewGuid())]));
    }

    [Fact]
    public void ValidateRegistration_WhenAnotherVehicleUsesIt_ThenRejectsDuplicate()
    {
        var proposed = VehicleWith(Guid.NewGuid());
        Assert.Throws<BusinessConflictException>(() => VehicleValidator.ValidateRegistrationNumber(
            proposed, [VehicleWith(Guid.NewGuid())]));
    }

    [Fact]
    public void ValidateIdentifiers_WhenUpdatingSameVehicle_ThenAllowsUnchangedValues()
    {
        var id = Guid.NewGuid();
        var proposed = VehicleWith(id);
        VehicleValidator.ValidateVin(proposed, [VehicleWith(id)]);
        VehicleValidator.ValidateRegistrationNumber(proposed, [VehicleWith(id)]);
    }
}
