namespace CreditWorks.VehicleManagement.Domain.Queries.Dto
{
    /// <summary>
    ///     Vehicle details returned by a read operation.
    /// </summary>
    /// <param name="Id">The vehicle identifier.</param>
    /// <param name="OwnerName">The owner's name.</param>
    /// <param name="Manufacturer">The manufacturer name.</param>
    /// <param name="Year">The year of manufacture.</param>
    /// <param name="Vin">The vehicle identification number.</param>
    /// <param name="RegistrationNumber">The registration number.</param>
    /// <param name="WeightKg">The weight in kilograms.</param>
    public sealed record VehicleDto(Guid Id, string OwnerName, 
        string Manufacturer, int Year, string Vin, string RegistrationNumber,
        decimal WeightKg)
    { }
}
