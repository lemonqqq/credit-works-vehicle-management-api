using CreditWorks.VehicleManagement.Application.Contracts;
using CreditWorks.VehicleManagement.Application.Models.Queries.Dto;
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
    private static readonly int _pageSize = 10;

    public async Task<Domain.Aggregates.Vehicle?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await context.Vehicles
            .Include(x => x.Manufacturer)
            .FirstOrDefaultAsync(vehicle => vehicle.Id == id, cancellationToken);
        return result == null ? null : VehicleMapper.ToAggregate(result);
    }

    public async Task<List<Domain.Aggregates.Vehicle>> QueryAsync(string? ownerName = null, string? manufacturer = null, int? year = null,
        string? vin = null, string? registrationNumber = null, decimal? minWeightKg = null, decimal? maxWeightKg = null,
        int pageNumber = 1, List<FieldSortOrder>? sortingList = null, CancellationToken cancellationToken = default)
    {
        var query = context.Vehicles
            .Include(x => x.Manufacturer)
            .Where(x => (string.IsNullOrWhiteSpace(ownerName) || x.OwnerName.Contains(ownerName))
                && (string.IsNullOrWhiteSpace(manufacturer) || x.Manufacturer.Name.Contains(manufacturer))
                && (year == null || x.Year == year)
                && (string.IsNullOrWhiteSpace(vin) || x.Vin.Contains(vin))
                && (string.IsNullOrWhiteSpace(registrationNumber) || x.RegistrationNumber.Contains(registrationNumber))
                && (!minWeightKg.HasValue || x.WeightKg >= minWeightKg.Value)
                && (!maxWeightKg.HasValue || x.WeightKg < maxWeightKg.Value));

        var orderedQuery = (sortingList == null
                ? query.OrderBy(x => x.OwnerName)
                : OrderBy(query, sortingList))
            .Skip(_pageSize * (pageNumber - 1))
            .Take(_pageSize);

        var result = await orderedQuery.ToListAsync(cancellationToken);
        return [..result.Select(VehicleMapper.ToAggregate)];
    }

    private static IOrderedQueryable<Vehicle> OrderBy(IQueryable<Vehicle> query, List<FieldSortOrder> fieldList)
    {
        var orderQuery = query.OrderBy(x => x.OwnerName);
        foreach (var item in fieldList)
        {
            if (item.FieldName == "ownerName" && !item.IsAscending)
                orderQuery = orderQuery.OrderByDescending(x => x.OwnerName);
            else if (item.FieldName == "manufacturer" && item.IsAscending)
                orderQuery = query.OrderBy(x => x.Manufacturer.Name);
            else if (item.FieldName == "manufacturer")
                orderQuery = query.OrderByDescending(x => x.Manufacturer.Name);
            else if (item.FieldName == "year" && item.IsAscending)
                orderQuery = query.OrderBy(x => x.Year);
            else if (item.FieldName == "year")
                orderQuery = query.OrderByDescending(x => x.Year);
            else if (item.FieldName == "weight" && item.IsAscending)
                orderQuery = query.OrderBy(x => x.WeightKg);
            else if (item.FieldName == "weight")
                orderQuery = query.OrderByDescending(x => x.WeightKg);
        }
        return orderQuery;
    }

    public async Task<int> QueryCountAsync(string? ownerName = null, string? manufacturer = null, int? year = null,
        string? vin = null, string? registrationNumber = null, decimal? minWeightKg = null, decimal? maxWeightKg = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Vehicles
            .Include(x => x.Manufacturer)
            .Where(x => (string.IsNullOrWhiteSpace(ownerName) || x.OwnerName.Contains(ownerName))
                && (string.IsNullOrWhiteSpace(manufacturer) || x.Manufacturer.Name.Contains(manufacturer))
                && (year == null || x.Year == year)
                && (string.IsNullOrWhiteSpace(vin) || x.Vin.Contains(vin))
                && (string.IsNullOrWhiteSpace(registrationNumber) || x.RegistrationNumber.Contains(registrationNumber))
                && (!minWeightKg.HasValue || x.WeightKg >= minWeightKg.Value)
                && (!maxWeightKg.HasValue || x.WeightKg < maxWeightKg.Value));

        return await query.CountAsync(cancellationToken);
    }

    public async Task AddAsync(Domain.Aggregates.Vehicle vehicle)
    {
        await context.Vehicles.AddAsync(VehicleMapper.ToEntity(vehicle));
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Domain.Aggregates.Vehicle vehicle)
    {
        var entity = await context.Vehicles
            .FirstAsync(x => x.Id == vehicle.Id);
        VehicleMapper.ApplyToTrackedEntity(vehicle, entity);
        context.Vehicles.Update(entity);
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
