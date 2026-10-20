using CreditWorks.VehicleManagement.Domain.Aggregates;

namespace CreditWorks.VehicleManagement.Application.Contracts
{
    /// <summary>
    ///     Defines persistence and query operations for vehicle categories.
    /// </summary>
    public interface ICategoryStore
    {
        /// <summary>
        ///     Gets a category by its identifier.
        /// </summary>
        /// <param name="id">The category identifier.</param>
        /// <param name="cancellationToken">Cancels the operation.</param>
        /// <returns>The category, or <see langword="null"/> if it does not exist.</returns>
        Task<VehicleCategory?> GetAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Gets all configured categories.
        /// </summary>
        /// <param name="cancellationToken">Cancels the operation.</param>
        /// <returns>The categories.</returns>
        Task<List<VehicleCategory>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///     Searches categories by name.
        /// </summary>
        /// <param name="name">The category name criterion.</param>
        /// <param name="cancellationToken">Cancels the operation.</param>
        /// <returns>The matching categories.</returns>
        Task<List<VehicleCategory>> QueryAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Atomically replaces the configured category set.
        /// </summary>
        /// <param name="categories">The complete validated category set.</param>
        /// <param name="cancellationToken">Cancels the database operation.</param>
        Task ReplaceAllAsync(IReadOnlyCollection<VehicleCategory> categories, CancellationToken cancellationToken);
    }
}
