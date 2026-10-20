using CreditWorks.VehicleManagement.Application.Models.Queries.Dto;
using CreditWorks.VehicleManagement.Domain.Aggregates;
using CreditWorks.VehicleManagement.Domain.Exceptions;

namespace CreditWorks.VehicleManagement.Application.Contracts;

/// <summary>
///     Defines persistence and query operations for <see cref="Vehicle"/> entities.
/// </summary>
public interface IVehicleStore
{
    /// <summary>
    ///     Retrieves a single vehicle by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the vehicle to load.</param>
    /// <param name="cancellationToken">A token used to cancel the request.</param>
    /// <returns>
    ///     The matching <see cref="Vehicle"/> instance, or <see langword="null"/> if no record is found.
    /// </returns>
    Task<Vehicle?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a list of vehicles that match the provided filter criteria.
    /// </summary>
    /// <param name="ownerName">Optional owner name fragment used to match vehicles whose owner name contains the supplied value.</param>
    /// <param name="manufacturer">Optional manufacturer name fragment used to match vehicles whose manufacturer name contains the supplied value.</param>
    /// <param name="year">Optional year used to match vehicles with an exact manufacturing year.</param>
    /// <param name="vin">Optional VIN fragment used to match vehicles whose VIN contains the supplied value.</param>
    /// <param name="registrationNumber">Optional registration number fragment used to match vehicles whose registration number contains the supplied value.</param>
    /// <param name="minWeightKg">Optional minimum vehicle weight in kilograms. Vehicles below this value are excluded.</param>
    /// <param name="maxWeightKg">Optional maximum vehicle weight in kilograms. Vehicles above this value are excluded.</param>
    /// <param name="pageNumber">Optional page number passed in</param>
    /// <param name="sortingList">Optional sorting list</param>
    /// <param name="cancellationToken">A token used to cancel the request.</param>
    /// <returns>
    ///     A list of vehicles that satisfy all specified filters. The related manufacturer is eagerly loaded.
    /// </returns>
    Task<List<Vehicle>> QueryAsync(string? ownerName = null, string? manufacturer = null, int? year = null,
        string? vin = null, string? registrationNumber = null, decimal? minWeightKg = null, decimal? maxWeightKg = null,
        int pageNumber = 1, List<FieldSortOrder>? sortingList = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves number of vehicles that match the provided filter criteria.
    /// </summary>
    /// <param name="ownerName">Optional owner name fragment used to match vehicles whose owner name contains the supplied value.</param>
    /// <param name="manufacturer">Optional manufacturer name fragment used to match vehicles whose manufacturer name contains the supplied value.</param>
    /// <param name="year">Optional year used to match vehicles with an exact manufacturing year.</param>
    /// <param name="vin">Optional VIN fragment used to match vehicles whose VIN contains the supplied value.</param>
    /// <param name="registrationNumber">Optional registration number fragment used to match vehicles whose registration number contains the supplied value.</param>
    /// <param name="minWeightKg">Optional minimum vehicle weight in kilograms. Vehicles below this value are excluded.</param>
    /// <param name="maxWeightKg">Optional maximum vehicle weight in kilograms. Vehicles above this value are excluded.</param>
    /// <param name="cancellationToken">A token used to cancel the request.</param>
    /// <returns>
    ///     Number of vehicles that satisfy all specified filters. The related manufacturer is eagerly loaded.
    /// </returns>
    Task<int> QueryCountAsync(string? ownerName = null, string? manufacturer = null, int? year = null,
        string? vin = null, string? registrationNumber = null, decimal? minWeightKg = null, decimal? maxWeightKg = null,
        CancellationToken cancellationToken = default);
    /// <summary>
    ///     Adds a new vehicle to the data store. The caller is responsible for saving the change.
    /// </summary>
    /// <param name="vehicle">The vehicle entity to insert.</param>
    Task AddAsync(Vehicle vehicle);

    /// <summary>
    ///     Updates an existing vehicle record in the data store.
    /// </summary>
    /// <param name="vehicle">The modified vehicle entity to persist.</param>
    Task UpdateAsync(Vehicle vehicle);

    /// <summary>
    ///     Deletes a vehicle from the data store by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the vehicle to remove.</param>
    /// <exception cref="ResourceNotFoundException">
    ///     Thrown when no vehicle exists for the supplied identifier.
    /// </exception>
    Task DeleteAsync(Guid id);
}
