namespace CreditWorks.VehicleManagement.Infrastructure.DataStore.Entities
{
    /// <summary>
    ///     A persisted vehicle weight category and its icon key.
    /// </summary>
    public sealed record VehicleCategory
    {
        /// <summary>
        ///     Gets or sets the category identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Gets or sets the category name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the category description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the lower weight boundary in kilograms.
        /// </summary>
        public decimal MinWeightKg { get; set; }

        /// <summary>
        ///     Gets or sets the upper weight boundary, or null for no upper bound.
        /// </summary>
        public decimal? MaxWeightKg { get; set; }

        /// <summary>
        ///     Gets or sets the category icon key.
        /// </summary>
        public string IconKey { get; set; }

        /// <summary>
        ///     Creates an empty category for persistence materialization.
        /// </summary>
        public VehicleCategory() { }

        /// <summary>
        ///     Creates a category with the supplied details.
        /// </summary>
        /// <param name="id">The category identifier.</param>
        /// <param name="name">The category name.</param>
        /// <param name="description">The category description.</param>
        /// <param name="minWeightKg">The lower weight boundary in kilograms.</param>
        /// <param name="maxWeightKg">The upper weight boundary, or null for no upper bound.</param>
        /// <param name="iconKey">The category icon key.</param>
        public VehicleCategory(Guid id, string name, string description, decimal minWeightKg, decimal? maxWeightKg, string iconKey)
        {
            Id = id;
            Name = name;
            Description = description;
            MinWeightKg = minWeightKg;
            MaxWeightKg = maxWeightKg;
            IconKey = iconKey;
        }
    }
}
