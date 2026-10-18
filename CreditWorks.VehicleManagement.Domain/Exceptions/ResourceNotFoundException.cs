namespace CreditWorks.VehicleManagement.Domain.Exceptions
{
    public sealed class ResourceNotFoundException(string message) : Exception(message);
}
