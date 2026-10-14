using CreditWorks.VehicleManagement.Application.Services;
using CreditWorks.VehicleManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CreditWorks.VehicleManagement.Infrastructure.Repositories;

public sealed class EfVehicleStore(VehicleManagementDbContext context) : IVehicleStore
{
    public Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        context.Vehicles.FirstOrDefaultAsync(vehicle => vehicle.Id == id, cancellationToken);

    public Task<Guid?> FindManufacturerIdAsync(string name, CancellationToken cancellationToken)
    {
        var normalizedName = name.ToUpperInvariant();
        return context.Manufacturers
            .Where(manufacturer => manufacturer.Name.ToUpper() == normalizedName)
            .Select(manufacturer => (Guid?)manufacturer.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> VinExistsAsync(string normalizedVin, Guid? exceptVehicleId, CancellationToken cancellationToken) =>
        context.Vehicles.AnyAsync(vehicle =>
            (!exceptVehicleId.HasValue || vehicle.Id != exceptVehicleId.Value) &&
            vehicle.Vin.Trim().ToUpper() == normalizedVin, cancellationToken);

    public Task<bool> RegistrationNumberExistsAsync(
        string normalizedRegistrationNumber, Guid? exceptVehicleId, CancellationToken cancellationToken) =>
        context.Vehicles.AnyAsync(vehicle =>
            (!exceptVehicleId.HasValue || vehicle.Id != exceptVehicleId.Value) &&
            vehicle.RegistrationNumber.Trim().ToUpper() == normalizedRegistrationNumber, cancellationToken);

    public async Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken) =>
        await context.Vehicles.AddAsync(vehicle, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken) =>
        await context.SaveChangesAsync(cancellationToken);
}
