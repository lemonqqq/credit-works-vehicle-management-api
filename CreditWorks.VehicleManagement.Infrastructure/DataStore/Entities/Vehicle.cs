namespace CreditWorks.VehicleManagement.Infrastructure.DataStore.Entities
{
    /// <summary>
    ///     A persisted vehicle and its manufacturer relationship.
    /// </summary>
    public sealed record Vehicle
    {
        /// <summary>
        ///     Gets or sets the vehicle identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Gets or sets the owner's name.
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        ///     Gets or sets the manufacturer identifier.
        /// </summary>
        public Guid ManufacturerId { get; set; }

        /// <summary>
        ///     Gets or sets the year of manufacture.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        ///     Gets or sets the vehicle identification number.
        /// </summary>
        public string Vin { get; set; }

        /// <summary>
        ///     Gets or sets the registration number.
        /// </summary>
        public string RegistrationNumber { get; set; }

        /// <summary>
        ///     Gets or sets the weight in kilograms.
        /// </summary>
        public decimal WeightKg { get; set; }

        /// <summary>
        ///     Creates a vehicle with the supplied details.
        /// </summary>
        /// <param name="id">The vehicle identifier.</param>
        /// <param name="ownerName">The owner's name.</param>
        /// <param name="manufacturerId">The manufacturer identifier.</param>
        /// <param name="year">The year of manufacture.</param>
        /// <param name="vin">The vehicle identification number.</param>
        /// <param name="registrationNumber">The registration number.</param>
        /// <param name="weightKg">The weight in kilograms.</param>
        public Vehicle(Guid id, string ownerName, Guid manufacturerId, int year, string vin, string registrationNumber, decimal weightKg)
        {
            Id = id;
            OwnerName = ownerName;
            ManufacturerId = manufacturerId;
            Year = year;
            Vin = vin;
            RegistrationNumber = registrationNumber;
            WeightKg = weightKg;
        }

        /// <summary>
        ///     Gets or sets the related manufacturer.
        /// </summary>
        public Manufacturer Manufacturer { get; set; }

        /// <summary>
        ///     Creates an empty vehicle for persistence materialization.
        /// </summary>
        public Vehicle() { }

    }
}
