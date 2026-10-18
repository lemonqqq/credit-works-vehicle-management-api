using CreditWorks.VehicleManagement.Domain.Aggregates;
using CreditWorks.VehicleManagement.Domain.Exceptions;

namespace CreditWorks.VehicleManagement.Application.Validators
{
    /// <summary>
    ///     Checks vehicle identifiers for duplicates among matching records.
    /// </summary>
    internal static class VehicleValidator
    {
        /// <summary>
        ///     Rejects a VIN already assigned to another vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle being validated.</param>
        /// <param name="matchingRecords">Vehicles matching its VIN.</param>
        /// <exception cref="BusinessConflictException">Another vehicle has the same VIN.</exception>
        public static void ValidateVin(Vehicle vehicle, List<Vehicle> matchingRecords)
        {
            if (matchingRecords.Any(x => x.Id != vehicle.Id))
                throw new BusinessConflictException($"{vehicle.Id} - same VIN ({vehicle.Vin}) with {matchingRecords.Count} records found");
        }

        /// <summary>
        ///     Rejects a registration number already assigned to another vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle being validated.</param>
        /// <param name="matchingRecords">Vehicles matching its registration number.</param>
        /// <exception cref="BusinessConflictException">Another vehicle has the same registration number.</exception>
        public static void ValidateRegistrationNumber(Vehicle vehicle, List<Vehicle> matchingRecords)
        {
            if (matchingRecords.Any(x => x.Id != vehicle.Id))
                throw new BusinessConflictException($"{vehicle.Id} - same registration number ({vehicle.Vin}) with {matchingRecords.Count} records found");
        }
    }
}
