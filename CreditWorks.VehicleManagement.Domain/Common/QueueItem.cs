namespace CreditWorks.VehicleManagement.Domain.Common
{
    public sealed record QueueItem(Guid Id, Func<IServiceProvider, Task> Run);
}
