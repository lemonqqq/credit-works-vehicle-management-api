using CreditWorks.VehicleManagement.Domain.Queries.Dto;

namespace CreditWorks.VehicleManagement.Domain.Queries
{
    /// <summary>
    ///     Vehicles returned by a search, with paging information.
    /// </summary>
    /// <param name="Data">The matching vehicles on the returned page.</param>
    /// <param name="PageCount">The number of pages.</param>
    /// <param name="TotalRecords">The total number of matching vehicles.</param>
    public sealed record VehicleSearchQueryResult(IReadOnlyList<VehicleDto> Data, int PageCount, int TotalRecords);
}
