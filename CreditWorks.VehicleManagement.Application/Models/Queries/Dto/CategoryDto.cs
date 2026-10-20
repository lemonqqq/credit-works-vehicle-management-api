namespace CreditWorks.VehicleManagement.Application.Models.Queries.Dto
{
    /// <summary>
    ///     Category details returned by a read operation.
    /// </summary>
    /// <param name="Id">The category identifier.</param>
    /// <param name="Name">The category name.</param>
    /// <param name="Description">The category description.</param>
    /// <param name="MinWeightKg">The lower weight boundary in kilograms.</param>
    /// <param name="MaxWeightKg">The upper weight boundary, or null for no upper bound.</param>
    /// <param name="IconKey">The category icon key.</param>
    public record CategoryDto(Guid Id, string Name, string Description, decimal MinWeightKg, decimal? MaxWeightKg, string IconKey);
}
