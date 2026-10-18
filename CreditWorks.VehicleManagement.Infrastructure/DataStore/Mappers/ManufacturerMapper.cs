using CreditWorks.VehicleManagement.Infrastructure.DataStore.Entities;
using AggregateManufacturer = CreditWorks.VehicleManagement.Domain.Aggregates.Manufacturer;

namespace CreditWorks.VehicleManagement.Infrastructure.DataStore.Mappers
{
    /// <summary>
    ///     Converts manufacturer between persistence, domain, and response models.
    /// </summary>
    internal static class ManufacturerMapper
    {
        /// <summary>
        ///     Rehydrates a domain category from a persisted category.
        /// </summary>
        /// <param name="entity">The persisted category.</param>
        /// <returns>The domain category.</returns>
        public static AggregateManufacturer ToAggregate(Manufacturer entity) =>
        AggregateManufacturer.Rehydrate(
            entity.Id,
            entity.Name);
    }
}
