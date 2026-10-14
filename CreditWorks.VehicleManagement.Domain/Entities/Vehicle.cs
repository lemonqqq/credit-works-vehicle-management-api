namespace CreditWorks.VehicleManagement.Domain.Entities
{
    public record Vehicle
    {
        public Guid Id { get; set; }

        public string OwnerName { get; set; }

        public Guid ManufacturerId { get; set; }

        public int Year { get; set; }

        public string Vin { get; set; }

        public string RegistrationNumber { get; set; }

        public decimal WeightKg { get; set; }

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

        public Manufacturer Manufacturer { get; set; }

        public Vehicle() { }

    }
}
