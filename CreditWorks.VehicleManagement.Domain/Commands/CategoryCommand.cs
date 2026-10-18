using System.ComponentModel.DataAnnotations;

namespace CreditWorks.VehicleManagement.Domain.Commands
{
    /// <summary>
    ///     A proposed complete set of vehicle categories.
    /// </summary>
    public sealed record CategoryCommand
    {
        /// <summary>
        ///     Gets or sets the categories to maintain.
        /// </summary>
        public List<CategoryDto> Categories { get; set; }
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
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets an optional description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the lower weight boundary in kilograms.
        /// </summary>
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Weight must be positive number")]
        public decimal MinWeightKg { get; set; }

        /// <summary>
        ///     Gets or sets the upper weight boundary, or null for no upper bound.
        /// </summary>
        public decimal? MaxWeightKg { get; set; }

        /// <summary>
        ///     Gets or sets the key of the category icon.
        /// </summary>
        [Required]
        public string IconKey { get; set; }
    }
}
