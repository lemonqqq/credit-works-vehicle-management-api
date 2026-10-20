using CreditWorks.VehicleManagement.Infrastructure.DataStore.Entities;
using AggregatesVehicleCategory = CreditWorks.VehicleManagement.Domain.Aggregates.VehicleCategory;

namespace CreditWorks.VehicleManagement.Infrastructure.DataStore.Mappers
{
    /// <summary>
    ///     Converts categories between persistence, domain, and response models.
    /// </summary>
    public static class VehicleCategoryMapper
    {
        /// <summary>
        ///     Rehydrates a domain category from a persisted category.
        /// </summary>
        /// <param name="entity">The persisted category.</param>
        /// <returns>The domain category.</returns>
        public static AggregatesVehicleCategory ToAggregate(VehicleCategory entity) =>
        AggregatesVehicleCategory.Rehydrate(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.MinWeightKg,
            entity.MaxWeightKg,
            entity.IconKey);

        /// <summary>
        ///     Creates a persistence entity from a domain category.
        /// </summary>
        /// <param name="category">The domain category.</param>
        /// <returns>The persistence entity.</returns>
        public static VehicleCategory ToEntity(AggregatesVehicleCategory category) =>
            new(
                category.Id,
                category.Name,
                category.Description,
                category.MinWeightKg,
                category.MaxWeightKg,
                category.IconKey);


        /// <summary>
        ///     Copies domain category values onto an existing tracked entity.
        /// </summary>
        /// <param name="category">The source domain category.</param>
        /// <param name="entity">The tracked entity to update.</param>
        /// <exception cref="InvalidOperationException">The category identifiers differ.</exception>
        public static void ApplyToTrackedEntity(
            AggregatesVehicleCategory category, VehicleCategory entity)
        {
            if (category.Id != entity.Id)
                throw new InvalidOperationException("Vehicle IDs do not match.");

            entity.Name = category.Name;
            entity.Description = category.Description;
            entity.MinWeightKg = category.MinWeightKg;
            entity.MaxWeightKg = category.MaxWeightKg;
            entity.IconKey = category.IconKey;
        }
    }
}
