using CreditWorks.VehicleManagement.Domain.Queries.Dto;
using AggregateVehicle = CreditWorks.VehicleManagement.Domain.Aggregates.Vehicle;

namespace CreditWorks.VehicleManagement.Application.Mappers
{
    /// <summary>
    ///     Converts vehicles between persistence, domain, and response models.
    /// </summary>
    internal static class VehicleMapper
    {
        /// <summary>
        ///     Creates a vehicle response model.
        /// </summary>
        /// <param name="vehicle">The persisted vehicle.</param>
        /// <returns>The vehicle response model.</returns>
        public static VehicleDto ToDto(AggregateVehicle vehicle)
            => new (vehicle.Id, vehicle.OwnerName, vehicle.ManufacturerName, vehicle.Year, vehicle.Vin,
                vehicle.RegistrationNumber, vehicle.WeightKg);

    }
}
