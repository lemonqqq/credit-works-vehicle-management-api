using CreditWorks.VehicleManagement.Application.Contracts;
using CreditWorks.VehicleManagement.Domain.Exceptions;
using CreditWorks.VehicleManagement.Infrastructure.DataStore.Entities;
using CreditWorks.VehicleManagement.Infrastructure.DataStore.Mappers;
using CreditWorks.VehicleManagement.Infrastructure.Ef;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace CreditWorks.VehicleManagement.Infrastructure.DataStore;

/// <summary>
///     Provides persistence and query operations for <see cref="Vehicle"/> entities using the configured
/// </summary>
/// <param name="context"></param>
public sealed class VehicleStore(VehicleManagementDbContext context) : IVehicleStore
{
    private static readonly int _pageSize = 100;

    public async Task<Domain.Aggregates.Vehicle?> GetAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var result = await context.Vehicles
            .Include(x => x.Manufacturer)
            .FirstOrDefaultAsync(vehicle => vehicle.Id == id, cancellationToken);
        return result == null ? null : VehicleMapper.ToAggregate(result);
    }

    public async Task<List<Domain.Aggregates.Vehicle>> QueryAsync(string? ownerName = null, string? manufacturer = null, int? year = null,
        string? vin = null, string? registrationNumber = null, decimal? minWeightKg = null, decimal? maxWeightKg = null,
        int pageNumber = 1, string? sortingList = null, CancellationToken cancellationToken = default)
    { 
        var result = await context.Vehicles
            .Include(x => x.Manufacturer)
            .Where(x => (string.IsNullOrWhiteSpace(ownerName) || x.OwnerName.Contains(ownerName))
                && (string.IsNullOrWhiteSpace(manufacturer) || x.Manufacturer.Name.Contains(manufacturer))
                && (year == null || x.Year == year)
                && (string.IsNullOrWhiteSpace(vin) || x.Vin.Contains(vin))
                && (string.IsNullOrWhiteSpace(registrationNumber) || x.RegistrationNumber.Contains(registrationNumber))
                && (!minWeightKg.HasValue || x.WeightKg >= minWeightKg.Value)
                && (!maxWeightKg.HasValue || x.WeightKg <= maxWeightKg.Value))
            .OrderBy(sortingList ?? "")
            .Skip(_pageSize * (pageNumber - 1))
            .Take(_pageSize)
            .ToListAsync(cancellationToken);
        return [.. result.Select(VehicleMapper.ToAggregate)];
    }

    public async Task AddAsync(Domain.Aggregates.Vehicle vehicle)
    {
        await context.Vehicles.AddAsync(VehicleMapper.ToEntity(vehicle));
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Domain.Aggregates.Vehicle vehicle)
    {
        context.Vehicles.Update(VehicleMapper.ToEntity(vehicle));
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var vehicle = await context.Vehicles.FirstOrDefaultAsync(vehicle => vehicle.Id == id)
            ?? throw new ResourceNotFoundException($"vehicle not found - {id}");

        context.Vehicles.Remove(vehicle);
        await context.SaveChangesAsync();
    }
}
