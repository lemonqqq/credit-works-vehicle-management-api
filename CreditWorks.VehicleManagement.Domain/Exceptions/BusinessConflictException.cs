namespace CreditWorks.VehicleManagement.Domain.Exceptions
{
    public sealed class BusinessConflictException(string message) : Exception(message);
}
