namespace CreditWorks.VehicleManagement.Infrastructure.DataStore.Entities
{
    /// <summary>
    ///     A predefined manufacturer that can be assigned to vehicles.
    /// </summary>
    public sealed record Manufacturer
    {
        /// <summary>
        ///     Gets or sets the manufacturer identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Gets or sets the manufacturer name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the vehicles associated with this manufacturer.
        /// </summary>
        public ICollection<Vehicle> Vehicles { get; set; }

        /// <summary>
        ///     Creates an empty manufacturer for persistence materialization.
        /// </summary>
        public Manufacturer() { }

        /// <summary>
        ///     Creates a manufacturer with an identifier and name.
        /// </summary>
        /// <param name="id">The manufacturer identifier.</param>
        /// <param name="name">The manufacturer name.</param>
        public Manufacturer(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
