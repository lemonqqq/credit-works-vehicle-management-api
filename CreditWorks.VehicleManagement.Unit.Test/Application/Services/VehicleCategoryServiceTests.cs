using CreditWorks.VehicleManagement.Application.Contracts;
using CreditWorks.VehicleManagement.Application.Models.Commands;
using CreditWorks.VehicleManagement.Application.Services.Impl;
using CreditWorks.VehicleManagement.Domain.Aggregates;
using CreditWorks.VehicleManagement.Domain.Exceptions;
using System.Collections.Immutable;

namespace CreditWorks.VehicleManagement.Unit.Test.Application.Services;

public sealed class VehicleCategoryServiceTests
{
    private static readonly Guid LightId = Guid.Parse("10000000-0000-3000-7000-000000000001");
    private static readonly Guid MediumId = Guid.Parse("10000000-0000-3000-7000-000000000002");
    private static readonly Guid HeavyId = Guid.Parse("10000000-0000-3000-7000-000000000003");

    private static List<VehicleCategory> Initial() =>
    [
        new(LightId, "Light", "Light vehicles", 0m, 500m, "car"),
        new(MediumId, "Medium", "Medium vehicles", 500m, 2500m, "van"),
        new(HeavyId, "Heavy", "Heavy vehicles", 2500m, null, "truck")
    ];

    private static CategoryCommand Proposal(IEnumerable<VehicleCategory> categories) => new()
    {
        Categories = categories.Select(x => new CategoryDto
        {
            Id = x.Id, Name = x.Name, Description = x.Description,
            MinWeightKg = x.MinWeightKg, MaxWeightKg = x.MaxWeightKg, IconKey = x.IconKey
        }).ToList()
    };

    [Fact]
    public async Task GetAll_WhenCategoriesConfigured_ThenReturnsIcons()
    {
        var service = new VehicleCategoryService(new CategoryStoreFake(Initial()), new CacheFake());
        var result = await service.GetAll(CancellationToken.None);
        Assert.Equal(["Light", "Medium", "Heavy"], result.Select(x => x.Name));
        Assert.Equal(["car", "van", "truck"], result.Select(x => x.IconKey));
    }

    [Fact]
    public async Task GetAll_WhenCalledAgain_ThenUsesCachedCategories()
    {
        var store = new CategoryStoreFake(Initial());
        var service = new VehicleCategoryService(store, new CacheFake());

        await service.GetAll(CancellationToken.None);
        var result = await service.GetAll(CancellationToken.None);

        Assert.Equal(1, store.GetAllCalls);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task Maintain_WhenSharedBoundaryMoves_ThenUpdatesAdjacentCategories()
    {
        var store = new CategoryStoreFake(Initial());
        var service = new VehicleCategoryService(store, new CacheFake());
        var proposal = Proposal(store.Data);
        proposal.Categories.Single(x => x.Id == MediumId).MaxWeightKg = 2000m;
        proposal.Categories.Single(x => x.Id == HeavyId).MinWeightKg = 2000m;

        Assert.Equal(3, await service.Maintain(proposal));
        Assert.Equal(2000m, store.Data.Single(x => x.Id == MediumId).MaxWeightKg);
        Assert.Equal(2000m, store.Data.Single(x => x.Id == HeavyId).MinWeightKg);
    }

    [Fact]
    public async Task Maintain_WhenDeletionCreatesGap_ThenKeepsPersistedCategories()
    {
        var store = new CategoryStoreFake(Initial());
        var service = new VehicleCategoryService(store, new CacheFake());
        var proposal = Proposal(store.Data.Where(x => x.Id != MediumId));

        var error = await Assert.ThrowsAsync<BusinessConflictException>(() => service.Maintain(proposal));
        Assert.Matches("(?i)gap|missing", error.Message);
        Assert.Equal(3, store.Data.Count);
    }

    [Fact]
    public async Task Maintain_WhenRangeSplitAndMerged_ThenCreatesAndDeletesCategory()
    {
        var store = new CategoryStoreFake(Initial());
        var service = new VehicleCategoryService(store, new CacheFake());
        var split = Proposal(store.Data);
        split.Categories.Single(x => x.Id == HeavyId).MaxWeightKg = 5000m;
        split.Categories.Add(new CategoryDto
        {
            Name = "Extra Heavy", Description = "Above 5000 kg", MinWeightKg = 5000m,
            MaxWeightKg = null, IconKey = "truck"
        });

        Assert.Equal(4, await service.Maintain(split));
        Assert.Contains(store.Data, x => x.Name == "Extra Heavy");

        var merge = Proposal(store.Data.Where(x => x.Name != "Extra Heavy"));
        merge.Categories.Single(x => x.Id == HeavyId).MaxWeightKg = null;
        Assert.Equal(3, await service.Maintain(merge));
        Assert.DoesNotContain(store.Data, x => x.Name == "Extra Heavy");
    }

    private sealed class CategoryStoreFake(List<VehicleCategory> initial) : ICategoryStore
    {
        public List<VehicleCategory> Data { get; } = initial;
        public int GetAllCalls { get; private set; }

        public Task<VehicleCategory?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = Data.SingleOrDefault(x => x.Id == id);
            return Task.FromResult(result == null ? null : result);
        }

        public Task<List<VehicleCategory>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            GetAllCalls++;
            return Task.FromResult(Data.ToList());
        }

        public Task<List<VehicleCategory>> QueryAsync(string name, CancellationToken cancellationToken = default) =>
            Task.FromResult(Data.Where(x => x.Name.Contains(name)).ToList());

        public Task ReplaceAllAsync(IReadOnlyCollection<VehicleCategory> categories, CancellationToken cancellationToken)
        {
            Data.Clear();
            Data.AddRange(categories);
            return Task.CompletedTask;
        }
    }

    private sealed class CacheFake : ICacheService
    {
        private readonly Dictionary<string, object> _values = [];

        public void Set<T>(IEnumerable<T> items, string cacheKey)
        {
            _values[cacheKey] = items.ToImmutableArray();
        }

        public bool TryGet<T>(string cacheKey, out ImmutableArray<T> items)
        {
            if (_values.TryGetValue(cacheKey, out var value) && value is ImmutableArray<T> cached)
            {
                items = cached;
                return true;
            }
            items = [];
            return false;
        }

        public void Remove(string cacheKey) => _values.Remove(cacheKey);
    }
}
