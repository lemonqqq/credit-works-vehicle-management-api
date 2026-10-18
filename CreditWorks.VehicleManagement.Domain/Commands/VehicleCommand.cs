using System.ComponentModel.DataAnnotations;

namespace CreditWorks.VehicleManagement.Domain.Commands
{
    /// <summary>
    ///     Vehicle details supplied to a create or update operation.
    /// </summary>
    public sealed record VehicleCommand : IValidatableObject
    {
        /// <summary>
        ///     Gets or sets the vehicle owner's name.
        /// </summary>
        [Required(ErrorMessage = "Missing vehicle owner name")]
        public string OwnerName { get; set; }

        /// <summary>
        ///     Gets or sets the predefined manufacturer name.
        /// </summary>
        [Required(ErrorMessage = "Missing vehicle manufacturer")]
        public string Manufacturer { get; set; }

        /// <summary>
        ///     Gets or sets the year of manufacture.
        /// </summary>
        [Required(ErrorMessage = "Missing vehicle make year")]
        [Range(2000, int.MaxValue, ErrorMessage = "Invalid vehicle make year")]
        public int Year { get; set; }

        /// <summary>
        ///     Gets or sets the vehicle identification number.
        /// </summary>
        [Required(ErrorMessage = "Missing vehicle vin")]
        [Length(17, 17, ErrorMessage = "Vin must have 17 characters")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Vin must be in alphanumeric")]
        public string Vin { get; set; }

        /// <summary>
        ///     Gets or sets the registration number.
        /// </summary>
        [Required(ErrorMessage = "Missing vehicle registration number")]
        [Length(1, 6, ErrorMessage = "Registration number must have 1 to 6 characters")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Registration number must be in alphanumeric")]
        public string RegistrationNumber { get; set; }

        /// <summary>
        ///     Gets or sets the vehicle weight in kilograms.
        /// </summary>
        [Required(ErrorMessage = "Missing vehicle weight")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be a positive number")]
        public decimal WeightKg { get; set; }

        /// <summary>
        ///     Checks rules that depend on the current year or decimal precision.
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Year > DateTime.UtcNow.Year + 1)
                yield return new ValidationResult("Year of manufacture is in the future", [nameof(Year)]);
            if (decimal.Round(WeightKg, 2) != WeightKg)
                yield return new ValidationResult("Weight supports at most two decimal places", [nameof(WeightKg)]);
        }
    }
}
