using CreditWorks.VehicleManagement.Domain.Aggregates.Enums;
using CreditWorks.VehicleManagement.Domain.Exceptions;

namespace CreditWorks.VehicleManagement.Domain.Aggregates
{
    public sealed class Vehicle
    {
        private const int MinYear = 2000;
        public Guid Id { get; private set; }

        public string OwnerName { get; private set; }

        public Guid? ManufacturerId { get; private set; }

        public string? ManufacturerName { get; private set; }

        public int Year { get; private set; }

        public string Vin { get; private set; }

        public string RegistrationNumber { get; private set; }

        public decimal WeightKg { get; private set; }

        public RecordStatus Status { get; private set; } = RecordStatus.Draft;

        public IList<VehicleValidationError> Errors => _errors;

        public Vehicle(Guid id, string ownerName, Guid? manufacturerId, int year,
            string vin, string registrationNumber, decimal weightKg, string? manufacturerName = null)
        {
            Validate(ownerName, manufacturerId, year, vin, registrationNumber, weightKg);
            Id = id;
            OwnerName = ownerName.Trim();
            ManufacturerId = manufacturerId;
            ManufacturerName = manufacturerName;
            Year = year;
            Vin = vin.Trim();
            RegistrationNumber = registrationNumber.Trim();
            WeightKg = weightKg;
        }

        public void Update(string ownerName, Guid? manufacturerId, int year,
            string vin, string registrationNumber, decimal weightKg, string? manufacturerName = null)
        {
            Validate(ownerName, manufacturerId, year, vin, registrationNumber, weightKg);
            OwnerName = ownerName.Trim();
            ManufacturerId = manufacturerId;
            ManufacturerName = manufacturerName ?? ManufacturerName;
            Year = year;
            Vin = vin.Trim();
            RegistrationNumber = registrationNumber.Trim();
            WeightKg = weightKg;
        }

        public static Vehicle Create(string ownerName, Guid? manufacturerId, int year,
            string vin, string registrationNumber, decimal weightKg)
            => new(Guid.NewGuid(), ownerName, manufacturerId, year, vin, registrationNumber, weightKg);

        // Rehydration preserves the persisted ID and manufacturer display name.
        public static Vehicle Rehydrate(Guid id, string ownerName, Guid manufacturerId, string manufacturerName, int year,
            string vin, string registrationNumber, decimal weightKg)
            => new(id, ownerName, manufacturerId, year, vin, registrationNumber, weightKg, manufacturerName);

        private IList<VehicleValidationError> _errors = [];

        private void Validate(string ownerName, Guid? manufacturerId, int year,
            string vin, string registrationNumber, decimal weightKg)
        {
            var errors = new List<VehicleValidationError>();
            if (string.IsNullOrWhiteSpace(ownerName))
                errors.Add(VehicleValidationError.MissingOrInvalidOwnerName);
            if (manufacturerId == null)
                errors.Add(VehicleValidationError.MismatchOrInvalidManufacturer);
            if (year < MinYear || year > DateTime.UtcNow.Year + 1)
                errors.Add(VehicleValidationError.InvalidYear);
            if (string.IsNullOrWhiteSpace(vin) || vin.Length != 17)
                errors.Add(VehicleValidationError.MissingOrInvalidVin);
            if (string.IsNullOrWhiteSpace(registrationNumber) || registrationNumber.Length > 6)
                errors.Add(VehicleValidationError.MissingOrInvalidRegistrationNumber);
            if (weightKg <= 0)
                errors.Add(VehicleValidationError.NegativeWeight);
            if (decimal.Round(weightKg, 2) != weightKg)
                errors.Add(VehicleValidationError.InvalidWeightPrecision);
            _errors = errors;
            if (errors.Count == 0) return;
            throw new BusinessValidationException(string.Join(",", errors));
        }
    }

    public enum VehicleValidationError
    {
        NoError,
        MissingOrInvalidOwnerName,
        MismatchOrInvalidManufacturer,
        InvalidYear,
        MissingOrInvalidVin,
        MissingOrInvalidRegistrationNumber,
        NegativeWeight,
        InvalidWeightPrecision
    }
}
