using System.ComponentModel.DataAnnotations;

namespace CreditWorks.VehicleManagement.Domain.Commands
{
    public record VehicleCommand
    {
        public required string OwnerName { get; set; }

        public required string Manufacturer { get; set; }

        public required int Year { get; set; }

        public required string Vin { get; set; }

        [Length(1, 6)]
        public required string RegistrationNumber { get; set; }

        public decimal WeightKg { get; set; }
    }
}
