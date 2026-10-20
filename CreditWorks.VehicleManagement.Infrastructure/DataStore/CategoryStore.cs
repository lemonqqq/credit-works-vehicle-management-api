using CreditWorks.VehicleManagement.Application.Contracts;
using CreditWorks.VehicleManagement.Domain.Exceptions;
using CreditWorks.VehicleManagement.Infrastructure.DataStore.Entities;
using CreditWorks.VehicleManagement.Infrastructure.DataStore.Mappers;
using CreditWorks.VehicleManagement.Infrastructure.Ef;
using Microsoft.EntityFrameworkCore;

namespace CreditWorks.VehicleManagement.Infrastructure.DataStore
{
    public class CategoryStore(VehicleManagementDbContext context) : ICategoryStore
    {
        public async Task<Domain.Aggregates.VehicleCategory?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await context.VehicleCategories.FirstOrDefaultAsync(category => category.Id == id, cancellationToken);
            return result == null ? null : VehicleCategoryMapper.ToAggregate(result);
        }

        public async Task<List<Domain.Aggregates.VehicleCategory>> GetAllAsync(CancellationToken cancellationToken = default)
        { 
            var result = await context.VehicleCategories.ToListAsync();
            return [..result.Select(VehicleCategoryMapper.ToAggregate)];
        }

        public async Task<List<Domain.Aggregates.VehicleCategory>> QueryAsync(string name, CancellationToken cancellationToken = default)
        { 
            var result = await context.VehicleCategories.Where(x => x.Name.Contains(name)).ToListAsync();
            return [.. result.Select(VehicleCategoryMapper.ToAggregate)];
        }

        public async Task ReplaceAllAsync(IReadOnlyCollection<Domain.Aggregates.VehicleCategory> categories, CancellationToken cancellationToken)
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            var existingCategories = await context.VehicleCategories.ToDictionaryAsync(x => x.Id, cancellationToken);

            foreach(var category in categories)
            {
                if (existingCategories.Remove(category.Id, out var tracked))
                {
                    VehicleCategoryMapper.ApplyToTrackedEntity(category, tracked);
                    continue;
                }
                await context.VehicleCategories.AddAsync(VehicleCategoryMapper.ToEntity(category), cancellationToken);
            }
            context.VehicleCategories.RemoveRange(existingCategories.Values);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
    }
}
