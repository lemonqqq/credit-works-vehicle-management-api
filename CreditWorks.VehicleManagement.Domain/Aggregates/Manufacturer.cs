namespace CreditWorks.VehicleManagement.Domain.Aggregates
{
    /// <summary>
    ///     A predefined manufacturer that can be assigned to vehicles.
    /// </summary>
    public sealed class Manufacturer
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

        public Manufacturer(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public static Manufacturer Rehydrate(Guid id, string name)
            => new(id, name);
    }
}
