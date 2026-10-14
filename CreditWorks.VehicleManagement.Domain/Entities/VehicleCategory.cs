namespace CreditWorks.VehicleManagement.Domain.Entities
{
    public record VehicleCategory
    {
        public required Guid Id { get; set; }

        public required string Name { get; set; }

        public string Description { get; set; }

        public required decimal MinWeightKg { get; set; }

        public required decimal MaxWeightKg { get; set; }

        public required string IconKey { get; set; }

        public VehicleCategory() { }

        public VehicleCategory(Guid id, string name, string description, decimal minWeightKg, decimal maxWeightKg, string iconKey)
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
