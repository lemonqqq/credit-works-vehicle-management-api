using CreditWorks.VehicleManagement.Domain.Commands;

namespace CreditWorks.VehicleManagement.Application.Services
{
    /// <summary>
    ///     Defines application operations for maintaining and reading vehicle categories.
    /// </summary>
    public interface IVehicleCategoryService
    {
        /// <summary>
        ///     Applies a category configuration.
        /// </summary>
        /// <param name="command">The proposed category set.</param>
        /// <param name="cancellationToken">Cancels the operation.</param>
        /// <returns>The number of categories maintained.</returns>
        Task<int> Maintain(CategoryCommand command, CancellationToken cancellationToken);

        /// <summary>
        ///     Validates the complete proposed category configuration.
        /// </summary>
        /// <param name="categories">All categories after the proposed change.</param>
        void Validate(List<CategoryDto> categories);

        /// <summary>
        ///     Gets all configured categories.
        /// </summary>
        /// <param name="cancellationToken">Cancels the operation.</param>
        /// <returns>The category details.</returns>
        Task<List<Domain.Queries.Dto.CategoryDto>> GetAll(CancellationToken cancellationToken);
    }
}
