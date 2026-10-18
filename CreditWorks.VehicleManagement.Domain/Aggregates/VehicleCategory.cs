using CreditWorks.VehicleManagement.Domain.Aggregates.Enums;
using CreditWorks.VehicleManagement.Domain.Exceptions;

namespace CreditWorks.VehicleManagement.Domain.Aggregates
{
    public sealed class VehicleCategory
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public decimal MinWeightKg { get; private set; }

        public decimal? MaxWeightKg { get; private set; }

        public string IconKey { get; private set; }

        public RecordStatus Status { get; private set; } = RecordStatus.Draft;

        public VehicleCategory(Guid id, string name, string description, decimal minWeightKg, 
            decimal? maxWeightKg, string iconKey)
        {
            Validate(name, minWeightKg, maxWeightKg, iconKey);
            Id = id;
            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            MinWeightKg = minWeightKg;
            MaxWeightKg = maxWeightKg;
            IconKey = iconKey.Trim();
        }

        // Used when loading an existing vehicle. It must preserve the database ID.
        public static VehicleCategory Rehydrate(Guid id, string name, string description, decimal minWeightKg,
            decimal? maxWeightKg, string iconKey)
            => new(id, name, description, minWeightKg, maxWeightKg, iconKey);

        public static VehicleCategory Create(string name, string description, decimal minWeightKg,
            decimal? maxWeightKg, string iconKey)
            => new(Guid.NewGuid(), name, description, minWeightKg, maxWeightKg, iconKey);

        public void Update(string name, string description, decimal? minWeightKg,
            decimal? maxWeightKg, string iconKey)
        {
            Validate(name, minWeightKg, maxWeightKg, iconKey);
            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            MinWeightKg = minWeightKg ?? 0;
            MaxWeightKg = maxWeightKg;
            IconKey = iconKey.Trim();
        }
        public IList<VehicleCategoryValidationError> Errors => _errors;
        private IList<VehicleCategoryValidationError> _errors = [];

        private void Validate(string name, decimal? minWeightKg,
            decimal? maxWeightKg, string iconKey)
        {
            var errors = new List<VehicleCategoryValidationError>();
            if (string.IsNullOrWhiteSpace(name))
                errors.Add(VehicleCategoryValidationError.MissingOrInvalidName);
            if (minWeightKg < 0 || minWeightKg >= (maxWeightKg ?? decimal.MaxValue))
                errors.Add(VehicleCategoryValidationError.InvalidMinWeight);
            if (minWeightKg >= (maxWeightKg ?? decimal.MaxValue))
                errors.Add(VehicleCategoryValidationError.InvalidMaxWeight);
            if (minWeightKg is { } lower && decimal.Round(lower, 2) != lower
                || maxWeightKg is { } upper && decimal.Round(upper, 2) != upper)
                errors.Add(VehicleCategoryValidationError.InvalidWeightPrecision);
            if (string.IsNullOrWhiteSpace(iconKey))
                errors.Add(VehicleCategoryValidationError.MissingIconKey);
            _errors = errors;
            if (errors.Count == 0) return;
            throw new BusinessValidationException(string.Join(",", errors));
        }
    }

    public enum VehicleCategoryValidationError
    {
        NoError,
        MissingOrInvalidName,
        InvalidMinWeight,
        InvalidMaxWeight,
        MissingIconKey,
        InvalidWeightPrecision
    }
}
