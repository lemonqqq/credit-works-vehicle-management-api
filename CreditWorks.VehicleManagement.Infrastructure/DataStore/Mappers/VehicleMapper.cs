using CreditWorks.VehicleManagement.Application.Models.Queries.Dto;
using CreditWorks.VehicleManagement.Infrastructure.DataStore.Entities;
using AggregatesVehicle = CreditWorks.VehicleManagement.Domain.Aggregates.Vehicle;

namespace CreditWorks.VehicleManagement.Infrastructure.DataStore.Mappers
{
    /// <summary>
    ///     Converts vehicles between persistence, domain, and response models.
    /// </summary>
    public static class VehicleMapper
    {
        /// <summary>
        ///     Rehydrates a domain vehicle from a persisted vehicle.
        /// </summary>
        /// <param name="entity">The persisted vehicle.</param>
        /// <returns>The domain vehicle.</returns>
        public static AggregatesVehicle ToAggregate(Vehicle entity) =>
        AggregatesVehicle.Rehydrate(
            entity.Id,
            entity.OwnerName,
            entity.ManufacturerId,
            entity.Manufacturer.Name,
            entity.Year,
            entity.Vin,
            entity.RegistrationNumber,
            entity.WeightKg);

        /// <summary>
        ///     Creates a persistence entity from a domain vehicle.
        /// </summary>
        /// <param name="vehicle">The domain vehicle.</param>
        /// <returns>The persistence entity.</returns>
        public static Vehicle ToEntity(AggregatesVehicle vehicle) =>
            new(
                vehicle.Id,
                vehicle.OwnerName,
                vehicle.ManufacturerId.Value,
                vehicle.Year,
                vehicle.Vin,
                vehicle.RegistrationNumber,
                vehicle.WeightKg);

        /// <summary>
        ///     Creates a vehicle response model.
        /// </summary>
        /// <param name="vehicle">The persisted vehicle.</param>
        /// <param name="manufacturerName">The manufacturer's display name.</param>
        /// <returns>The vehicle response model.</returns>
        public static VehicleDto ToDto(Vehicle vehicle, string manufacturerName)
            => new(vehicle.Id, vehicle.OwnerName, manufacturerName, vehicle.Year, vehicle.Vin,
                vehicle.RegistrationNumber, vehicle.WeightKg);

        /// <summary>
        ///     Copies domain vehicle values onto an existing tracked entity.
        /// </summary>
        /// <param name="vehicle">The source domain vehicle.</param>
        /// <param name="entity">The tracked entity to update.</param>
        /// <exception cref="InvalidOperationException">The vehicle identifiers differ.</exception>
        public static void ApplyToTrackedEntity(
            AggregatesVehicle vehicle, Vehicle entity)
        {
            if (vehicle.Id != entity.Id)
                throw new InvalidOperationException("Vehicle IDs do not match.");

            entity.OwnerName = vehicle.OwnerName;
            entity.ManufacturerId = vehicle.ManufacturerId.Value;
            entity.Year = vehicle.Year;
            entity.Vin = vehicle.Vin;
            entity.RegistrationNumber = vehicle.RegistrationNumber;
            entity.WeightKg = vehicle.WeightKg;

        }
    }
}