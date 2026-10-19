using CreditWorks.VehicleManagement.Application.Models.Queries.Dto;
using System.ComponentModel.DataAnnotations;

namespace CreditWorks.VehicleManagement.Application.Models.Queries
{
    /// <summary>
    ///     Optional criteria for searching vehicles.
    /// </summary>
    public sealed record VehicleSearchQuery : IValidatableObject
    {
        /// <summary>
        ///     Gets or sets search text for owner name, registration or VIN.
        /// </summary>
        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "Owner name must be in alphanumeric")]
        public string? OwnerName { get; set; }

        /// <summary>
        ///     Gets or sets a manufacturer name filter.
        /// </summary>
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Manufacturer name field must be in alphanumeric")]
        public string? ManufacturerName { get; set; }

        /// <summary>
        ///     Gets or sets a year of manufacture filter.
        /// </summary>
        [Range(2000, 2050, ErrorMessage = "Invalid vehicle make year")]
        public int? Year { get; set; }

        /// <summary>
        ///     Gets or sets the minimum weight in kilograms.
        /// </summary>
        public decimal? MinWeightKg { get; set; }

        /// <summary>
        ///     Gets or sets the maximum weight in kilograms.
        /// </summary>
        public decimal? MaxWeightKg { get; set; }

        /// <summary>
        ///     Gets or sets the list of fields and their sort order.
        /// </summary>
        /// <remarks>Null if no sort fields are specified.</remarks>
        public List<FieldSortOrder>? SortFieldList { get; set; }

        /// <summary>
        ///     Gets or sets the requested page number.
        /// </summary>
        public int PageNumber { get; set; } = 1;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PageNumber < 1)
                yield return new ValidationResult("Page number must be positive", [nameof(PageNumber)]);
            if (MinWeightKg < 0 || MaxWeightKg < 0)
                yield return new ValidationResult("Weight filters cannot be negative", [nameof(MinWeightKg), nameof(MaxWeightKg)]);
            if (MinWeightKg.HasValue && MaxWeightKg.HasValue && MinWeightKg >= MaxWeightKg)
                yield return new ValidationResult("Maximum weight must exceed minimum weight", [nameof(MinWeightKg), nameof(MaxWeightKg)]);
            var permittedFields = new HashSet<string>(StringComparer.Ordinal)
                { "ownerName", "manufacturer", "year", "weight" };
            if (SortFieldList?.Any(x => x is null || !permittedFields.Contains(x.FieldName ?? "")) == true)
                yield return new ValidationResult("Unsupported sort field", [nameof(SortFieldList)]);
        }
    }
}
