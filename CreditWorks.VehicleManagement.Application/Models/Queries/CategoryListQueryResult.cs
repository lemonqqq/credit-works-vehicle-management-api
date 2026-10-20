using CreditWorks.VehicleManagement.Application.Models.Queries.Dto;

namespace CreditWorks.VehicleManagement.Application.Models.Queries
{
    /// <summary>
    ///     Categories returned by a list query.
    /// </summary>
    /// <param name="Data">The category details.</param>
    public sealed record CategoryListQueryResult(IEnumerable<CategoryDto> Data)
    { }
}
