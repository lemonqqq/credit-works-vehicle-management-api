using CreditWorks.VehicleManagement.Domain.Queries.Dto;

namespace CreditWorks.VehicleManagement.Domain.Queries
{
    /// <summary>
    ///     Categories returned by a list query.
    /// </summary>
    /// <param name="Data">The category details.</param>
    public sealed record CategoryListQueryResult(IEnumerable<CategoryDto> Data)
    { }
}
