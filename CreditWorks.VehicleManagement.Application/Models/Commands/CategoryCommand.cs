using CreditWorks.VehicleManagement.Application.Validators;
using CreditWorks.VehicleManagement.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace CreditWorks.VehicleManagement.Application.Models.Commands
{
    /// <summary>
    ///     A proposed complete set of vehicle categories.
    /// </summary>
    public sealed record CategoryCommand : IValidatableObject
    {
        /// <summary>
        ///     Gets or sets the categories to maintain.
        /// </summary>
        public List<CategoryDto> Categories { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (Categories is null || Categories.Count == 0)
                return [new ValidationResult(
                "At least one category is required.",
                [nameof(Categories)])];

            if (Categories.Any(category => category is null))
                return [new ValidationResult(
                "Categories cannot contain null items.",
                [nameof(Categories)])];

            try
            {
                VehicleCategoryValidator.Validate(Categories);
                return [];
            }
            catch (BusinessConflictException ex)
            {
                return [new ValidationResult(ex.Message, [nameof(Categories)])];
            }
        }
    }

    /// <summary>
    ///     Category details supplied by a category maintenance request.
    /// </summary>
    public sealed record CategoryDto
    {
        /// <summary>
        ///     Gets or sets the identifier of an existing category, if any.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        ///     Gets or sets the category name.
        /// </summary>
        [Required]
        [Length(1, 100, ErrorMessage = "Category must have 1 to 100 characters")]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets an optional description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the lower weight boundary in kilograms.
        /// </summary>
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Minimum must be positive number")]
        public decimal MinWeightKg { get; set; }

        /// <summary>
        ///     Gets or sets the upper weight boundary, or null for no upper bound.
        /// </summary>
        public decimal? MaxWeightKg { get; set; }

        /// <summary>
        ///     Gets or sets the key of the category icon.
        /// </summary>
        [Required]
        [Length(1, 100, ErrorMessage = "IconKey must have 1 to 100 characters")]
        [RegularExpression(@"^(bike|car|van|truck)$", ErrorMessage = "Unsupported category icon")]
        public string IconKey { get; set; }

    }

}
