using CreditWorks.VehicleManagement.Application.Contracts;
using CreditWorks.VehicleManagement.Infrastructure.DataStore.Mappers;
using CreditWorks.VehicleManagement.Infrastructure.Ef;
using Microsoft.EntityFrameworkCore;

namespace CreditWorks.VehicleManagement.Infrastructure.DataStore
{
    public class ManufacturerStore(VehicleManagementDbContext context) : IManufacturerStore
    {
        public async Task<Domain.Aggregates.Manufacturer?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {  
            var result = await context.Manufacturers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return result == null ? null : ManufacturerMapper.ToAggregate(result);
        }

        public async Task<Domain.Aggregates.Manufacturer?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        { 
            var result = await context.Manufacturers.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
            return result == null ? null : ManufacturerMapper.ToAggregate(result);
        }

        public async Task<List<Domain.Aggregates.Manufacturer>> GetAllAsync(CancellationToken cancellationToken)
        { 
            var result = await context.Manufacturers.ToListAsync(cancellationToken);
            return [.. result.Select(ManufacturerMapper.ToAggregate)];
        }
    }
}
