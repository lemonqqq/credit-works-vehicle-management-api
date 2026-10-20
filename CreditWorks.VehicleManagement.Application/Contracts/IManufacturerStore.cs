using CreditWorks.VehicleManagement.Domain.Aggregates;

namespace CreditWorks.VehicleManagement.Application.Contracts
{
    /// <summary>
    ///     Provides read access to predefined manufacturers.
    /// </summary>
    public interface IManufacturerStore
    {
        /// <summary>
        ///     Gets a manufacturer by its identifier.
        /// </summary>
        /// <param name="id">The manufacturer identifier.</param>
        /// <param name="cancellationToken">Cancels the operation.</param>
        /// <returns>The manufacturer, or <see langword="null"/> if it does not exist.</returns>
        Task<Manufacturer?> GetAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Gets a manufacturer by name.
        /// </summary>
        /// <param name="name">The manufacturer name to match.</param>
        /// <param name="cancellationToken">Cancels the operation.</param>
        /// <returns>The manufacturer, or <see langword="null"/> if it does not exist.</returns>
        Task<Manufacturer?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Gets all configured manufacturers.
        /// </summary>
        /// <param name="cancellationToken">Cancels the operation.</param>
        /// <returns>The manufacturers.</returns>
        Task<List<Manufacturer>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
