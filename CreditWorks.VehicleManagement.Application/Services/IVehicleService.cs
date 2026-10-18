using CreditWorks.VehicleManagement.Domain.Aggregates;
using CreditWorks.VehicleManagement.Domain.Commands;
using CreditWorks.VehicleManagement.Domain.Queries;
using CreditWorks.VehicleManagement.Domain.Queries.Dto;

namespace CreditWorks.VehicleManagement.Application.Services
{
    /// <summary>
    ///     Defines application operations for creating, updating, and reading vehicles.
    /// </summary>
    public interface IVehicleService
    {
        /// <summary>
        ///     Validates a command and prepares the vehicle that will be persisted.
        /// </summary>
        /// <param name="command">The proposed vehicle details.</param>
        /// <param name="id">An existing vehicle identifier for an update.</param>
        /// <returns>The validated vehicle aggregate.</returns>
        Task<Vehicle> Validate(VehicleCommand command, Guid? id = null);

        /// <summary>
        ///     Creates a vehicle from the supplied details.
        /// </summary>
        /// <param name="vehicle">The vehicle details to create.</param>
        /// <returns>A task that completes when the vehicle has been saved.</returns>
        Task Create(Vehicle vehicle);

        /// <summary>
        ///     Updates an existing vehicle.
        /// </summary>
        /// <param name="vehicle">The replacement vehicle details.</param>
        /// <returns>A task that completes when the vehicle has been saved.</returns>
        Task Update(Vehicle vehicle);

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
        /// <param name="orderFields">The list of order of the fields</param>
        /// <param name="cancellationToken">Cancels the operation.</param>
        /// <returns>The matching vehicles.</returns>
        Task<IEnumerable<VehicleDto>> Search(VehicleSearchQuery query,
            List<FieldSortOrder> orderFields = null, CancellationToken cancellationToken = default);
    }
}
