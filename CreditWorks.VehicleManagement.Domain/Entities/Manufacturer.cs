namespace CreditWorks.VehicleManagement.Domain.Entities
{
    public sealed record Manufacturer
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; }

        public Manufacturer() { }

        public Manufacturer(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
