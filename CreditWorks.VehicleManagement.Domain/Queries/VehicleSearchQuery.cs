namespace CreditWorks.VehicleManagement.Domain.Queries
{
    /// <summary>
    ///     Optional criteria for searching vehicles.
    /// </summary>
    public sealed record VehicleSearchQuery
    {
        /// <summary>
        ///     Gets or sets an owner name filter.
        /// </summary>
        public string? OwnerName { get; set; }

        /// <summary>
        ///     Gets or sets a manufacturer name filter.
        /// </summary>
        public string? ManufacturerName { get; set; }

        /// <summary>
        ///     Gets or sets a year of manufacture filter.
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        ///     Gets or sets a vehicle identification number filter.
        /// </summary>
        public string? Vin { get; set; }

        /// <summary>
        ///     Gets or sets a registration number filter.
        /// </summary>
        public string? RegistrationNumber { get; set; }

        /// <summary>
        ///     Gets or sets the minimum weight in kilograms.
        /// </summary>
        public decimal? MinWeightKg { get; set; }

        /// <summary>
        ///     Gets or sets the maximum weight in kilograms.
        /// </summary>
        public decimal? MaxWeightKg { get; set; }

        /// <summary>
        ///     Gets or sets the requested page number.
        /// </summary>
        public int PageNumber { get; set; } = 1;
    }
}
