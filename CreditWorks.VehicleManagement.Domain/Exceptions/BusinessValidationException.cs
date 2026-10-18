namespace CreditWorks.VehicleManagement.Domain.Exceptions
{
    public sealed class BusinessValidationException(string message) : Exception(message);
}
