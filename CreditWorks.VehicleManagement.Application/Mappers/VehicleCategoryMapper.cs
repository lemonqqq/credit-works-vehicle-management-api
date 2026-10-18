using CreditWorks.VehicleManagement.Domain.Aggregates;
using CreditWorks.VehicleManagement.Domain.Queries.Dto;

namespace CreditWorks.VehicleManagement.Application.Mappers
{
    /// <summary>
    ///     Converts categories between persistence, domain, and response models.
    /// </summary>
    public static class VehicleCategoryMapper
    {
        /// <summary>
        ///     Creates a category response model.
        /// </summary>
        /// <param name="category">The domain category.</param>
        /// <returns>The category response model.</returns>
        public static CategoryDto ToDto(VehicleCategory category)
            => new(category.Id, category.Name, category.Description, category.MinWeightKg, 
                category.MaxWeightKg, category.IconKey);
    }
}
