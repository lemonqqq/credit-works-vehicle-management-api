using CreditWorks.VehicleManagement.Application.Models.Commands;
using CreditWorks.VehicleManagement.Application.Models.Queries;
using CreditWorks.VehicleManagement.Application.Models.Queries.Dto;

namespace CreditWorks.VehicleManagement.Application.Services
{
    /// <summary>
    ///     Defines application operations for creating, updating, and reading vehicles.
    /// </summary>
    public interface IVehicleService
    {
        /// <summary>
        ///     Creates a vehicle from the supplied details.
        /// </summary>
        /// <param name="command">The proposed vehicle details.</param>
        /// <returns>A task that completes when the vehicle has been saved.</returns>
        Task Create(VehicleCommand command);

        /// <summary>
        ///     Updates an existing vehicle.
        /// </summary>
        /// <param name="command">The proposed vehicle details.</param>
        /// <param name="id">An existing vehicle identifier for an update.</param>
        /// <returns>A task that completes when the vehicle has been saved.</returns>
        Task Update(VehicleCommand command, Guid id);

        /// <summary>
        ///     Gets a vehicle by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the vehicle to retrieve.</param>
        /// <param name="cancellationToken">Cancels the operation.</param>
        /// <returns>The vehicle details.</returns>
        Task<VehicleDto> Get(Guid id, CancellationToken cancellationToken);

        /// <summary>
        ///     Finds vehicles matching the supplied search criteria.
        /// </summary>
        /// <param name="query">The criteria used to select vehicles.</param>
        /// <param name="cancellationToken">Cancels the operation.</param>
        /// <returns>The matching vehicles.</returns>
        Task<VehicleSearchQueryResult> Search(VehicleSearchQuery query, CancellationToken cancellationToken = default);
    }
}
