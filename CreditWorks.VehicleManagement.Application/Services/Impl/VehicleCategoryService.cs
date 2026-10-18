using CreditWorks.VehicleManagement.Application.Contracts;
using CreditWorks.VehicleManagement.Application.Mappers;
using CreditWorks.VehicleManagement.Application.Validators;
using CreditWorks.VehicleManagement.Domain.Aggregates;
using CreditWorks.VehicleManagement.Domain.Commands;

namespace CreditWorks.VehicleManagement.Application.Services.Impl
{
    public class VehicleCategoryService(ICategoryStore categoryStore, 
        ICacheService cacheService) : IVehicleCategoryService
    {
        private const string CategoryCacheKey = "VehicleCategoryCache";

        public async Task<List<Domain.Queries.Dto.CategoryDto>> GetAll(CancellationToken cancellationToken = default)
        {
            if (cacheService.TryGet<VehicleCategory>(CategoryCacheKey, out var categoryCache))
            {
                return [.. categoryCache.Select(VehicleCategoryMapper.ToDto)];
            }
            var categories = await categoryStore.GetAllAsync(cancellationToken);
            cacheService.Set(categories, CategoryCacheKey);
            return [.. categories.Select(VehicleCategoryMapper.ToDto)];
        }

        public void Validate(List<CategoryDto> categories)
        {
            VehicleCategoryValidator.Validate(categories);
        }

        public async Task<int> Maintain(CategoryCommand command, CancellationToken cancellationToken = default)
        {
            // A queued request must still satisfy the complete-range rules when it executes.
            Validate(command.Categories);
            var categories = command.Categories.Select(x =>
            {
                if (x.Id == null)
                    return VehicleCategory.Create(x.Name, x.Description, x.MinWeightKg, x.MaxWeightKg,
                        x.IconKey);
                return VehicleCategory.Rehydrate(x.Id.Value, x.Name, x.Description, x.MinWeightKg, x.MaxWeightKg,
                    x.IconKey);
            }).ToList();

            await categoryStore.ReplaceAllAsync(categories, cancellationToken);
            cacheService.Remove(CategoryCacheKey);
            return categories.Count;
        }
    }
}
