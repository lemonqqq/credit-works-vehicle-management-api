using CreditWorks.VehicleManagement.Application.Models.Queries.Dto;

namespace CreditWorks.VehicleManagement.Application.Models.Queries
{
    /// <summary>
    ///     Vehicles returned by a search, with paging information.
    /// </summary>
    /// <param name="Data">The matching vehicles on the returned page.</param>
    /// <param name="PageNumber">The requested page number.</param>
    /// <param name="TotalRecords">The total number of matching vehicles.</param>
    public sealed record VehicleSearchQueryResult(IReadOnlyList<VehicleDto> Data, int PageNumber, int TotalRecords);
}
