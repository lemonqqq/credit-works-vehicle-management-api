namespace CreditWorks.VehicleManagement.Domain.Entities
{
    public record VehicleCategory
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal MinWeightKg { get; set; }

        public decimal? MaxWeightKg { get; set; }

        public string IconKey { get; set; }

        public VehicleCategory() { }

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
