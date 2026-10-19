using System.ComponentModel.DataAnnotations;

namespace CreditWorks.VehicleManagement.Application.Models.Commands
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
        [Length(1, 250, ErrorMessage = "Owner name must have 1 to 250 characters")]
        [RegularExpression(@"^[a-zA-Z0-9 ]*$", ErrorMessage = "Owner name must contain only letters, digits and spaces")]
        public string OwnerName { get; set; }

        /// <summary>
        ///     Gets or sets the predefined manufacturer name.
        /// </summary>
        [Required(ErrorMessage = "Missing vehicle manufacturer")]
        [Length(1, 100, ErrorMessage = "Manufacturer must have 1 to 100 characters")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Manufacturer must contain only letters, digits and spaces")]
        public string Manufacturer { get; set; }

        /// <summary>
        ///     Gets or sets the year of manufacture.
        /// </summary>
        [Required(ErrorMessage = "Missing vehicle make year")]
        [Range(2000, 2050, ErrorMessage = "Invalid vehicle make year")]
        public int Year { get; set; }

        /// <summary>
        ///     Gets or sets the vehicle identification number.
        /// </summary>
        [Required(ErrorMessage = "Missing vehicle VIN")]
        [Length(17, 17, ErrorMessage = "VIN must have 17 characters")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "VIN must contain only letters and digits")]
        public string Vin { get; set; }

        /// <summary>
        ///     Gets or sets the registration number.
        /// </summary>
        [Required(ErrorMessage = "Missing vehicle registration number")]
        [Length(1, 6, ErrorMessage = "Registration number must have 1 to 6 characters")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Registration number must contain only letters and digits")]
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
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (Year > DateTime.UtcNow.Year + 1)
                yield return new ValidationResult("Year of manufacture is in the future", [nameof(Year)]);
            if (decimal.Round(WeightKg, 2) != WeightKg)
                yield return new ValidationResult("Weight supports at most two decimal places", [nameof(WeightKg)]);            
        }
    }
}
