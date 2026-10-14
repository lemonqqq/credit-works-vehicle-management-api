namespace CreditWorks.VehicleManagement.Domain.Entities
{
    public record Vehicle
    {
        public Guid Id { get; set; }

        public required string OwnerName { get; set; }

        public required string Manufacturer { get; set; }

        public required int Year { get; set; }

        public required string Vin { get; set; }

        public required string RegistrationNumber { get; set; }

        public decimal WeightKg { get; set; }

        public Vehicle(Guid id, string ownerName, string manufacturer, int year, string vin, string registrationNumber, decimal weightKg)
        {
            Id = id;
            OwnerName = ownerName;
            Manufacturer = manufacturer;
            Year = year;
            Vin = vin;
            RegistrationNumber = registrationNumber;
            WeightKg = weightKg;
        }

        public Vehicle() { }

    }
}
