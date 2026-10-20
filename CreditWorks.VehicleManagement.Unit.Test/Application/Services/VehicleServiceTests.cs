using CreditWorks.VehicleManagement.Application.Contracts;
using CreditWorks.VehicleManagement.Application.Models.Commands;
using CreditWorks.VehicleManagement.Application.Models.Queries;
using CreditWorks.VehicleManagement.Application.Models.Queries.Dto;
using CreditWorks.VehicleManagement.Application.Services.Impl;
using CreditWorks.VehicleManagement.Domain.Aggregates;
using CreditWorks.VehicleManagement.Domain.Exceptions;

namespace CreditWorks.VehicleManagement.Unit.Test.Application.Services;

public sealed class VehicleServiceTests
{
    private static readonly Guid MazdaId = Guid.Parse("10000000-0000-5000-9000-000000000001");

    private static VehicleCommand ValidCommand() => new()
    {
        OwnerName = "Alex Smith", Manufacturer = "Mazda", Year = 2020,
        WeightKg = 1850.75m, Vin = "ABC12345678901234", RegistrationNumber = "ABC123"
    };

    private static VehicleService CreateService(VehicleStoreFake vehicles) =>
        new(vehicles, new ManufacturerStoreFake(), new LoggerFake());

    [Fact]
    public async Task Validate_WhenIdentifiersMissing_ThenRejectsCurrentVehicleContract()
    {
        var store = new VehicleStoreFake();
        var command = ValidCommand();
        command.Vin = null!;
        command.RegistrationNumber = null!;

        await Assert.ThrowsAsync<BusinessValidationException>(
            () => CreateService(store).Create(command));
        Assert.Empty(store.Data);
    }

    [Fact]
    public async Task Create_WhenValidated_ThenAddsVehicle()
    {
        var store = new VehicleStoreFake();
        var service = CreateService(store);
        await service.Create(ValidCommand());
        Assert.NotEqual(Guid.Empty, Assert.Single(store.Data).Id);
        Assert.Equal(1850.75m, store.Data[0].WeightKg);
        Assert.Equal(MazdaId, store.Data[0].ManufacturerId);
    }

    [Fact]
    public async Task Update_WhenDetailsChange_ThenPersistsProposedValues()
    {
        var store = new VehicleStoreFake();
        var existing = Vehicle.Rehydrate(Guid.NewGuid(), "Alex Smith", MazdaId, "Mazda", 2020,
            "ABC12345678901234", "ABC123", 1850.75m);
        store.Data.Add(existing);
        var command = ValidCommand();
        command.OwnerName = "Taylor Smith";
        command.WeightKg = 499.99m;

        var service = CreateService(store);
        await service.Update(command, existing.Id);

        Assert.Equal("Taylor Smith", store.Data[0].OwnerName);
        Assert.Equal(499.99m, store.Data[0].WeightKg);
    }

    [Fact]
    public async Task Search_WhenNoFiltersProvided_ThenReturnsAllVehicles()
    {
        var store = new VehicleStoreFake();
        store.Data.Add(Vehicle.Rehydrate(Guid.NewGuid(), "Alex Smith", MazdaId, "Mazda", 2020,
            "ABC12345678901234", "ABC123", 1850.75m));

        var result = await CreateService(store).Search(new VehicleSearchQuery(), CancellationToken.None);
        Assert.Single(result.Data);
        Assert.Equal("Mazda", result.Data[0].Manufacturer);
    }

    [Fact]
    public async Task Search_WhenNoVehiclesMatch_ThenReturnsEmptyList()
    {
        var result = await CreateService(new VehicleStoreFake())
            .Search(new VehicleSearchQuery(), CancellationToken.None);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Get_WhenVehicleExists_ThenReturnsVehicle()
    {
        var store = new VehicleStoreFake();
        var vehicle = Vehicle.Rehydrate(Guid.NewGuid(), "Alex Smith", MazdaId, "Mazda", 2020,
            "ABC12345678901234", "ABC123", 1850.75m);
        store.Data.Add(vehicle);

        var result = await CreateService(store).Get(vehicle.Id, CancellationToken.None);
        Assert.Equal(vehicle.Id, result.Id);
        Assert.Equal("Mazda", result.Manufacturer);
    }

    [Fact]
    public async Task Create_WhenManufacturerUnknown_ThenDoesNotPersistVehicle()
    {
        var store = new VehicleStoreFake();
        var command = ValidCommand();
        command.Manufacturer = "Unknown";
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => CreateService(store).Create(command));
        Assert.Empty(store.Data);
    }

    [Fact]
    public async Task Create_WhenVinAlreadyUsed_ThenDoesNotPersistVehicle()
    {
        var store = new VehicleStoreFake();
        store.Data.Add(Vehicle.Rehydrate(Guid.NewGuid(), "Existing", MazdaId, "Mazda", 2020,
            "ABC12345678901234", "OLD123", 100m));
        await Assert.ThrowsAsync<BusinessConflictException>(() => CreateService(store).Create(ValidCommand()));
        Assert.Single(store.Data);
    }

    [Fact]
    public async Task Create_WhenRegistrationAlreadyUsed_ThenDoesNotPersistVehicle()
    {
        var store = new VehicleStoreFake();
        store.Data.Add(Vehicle.Rehydrate(Guid.NewGuid(), "Existing", MazdaId, "Mazda", 2020,
            "XYZ12345678901234", "ABC123", 100m));
        await Assert.ThrowsAsync<BusinessConflictException>(() => CreateService(store).Create(ValidCommand()));
        Assert.Single(store.Data);
    }

    [Fact]
    public async Task Create_WhenRegistrationIsOnlyAStringFragment_ThenAddsVehicle()
    {
        var store = new VehicleStoreFake();
        store.Data.Add(Vehicle.Rehydrate(Guid.NewGuid(), "Existing", MazdaId, "Mazda", 2020,
            "XYZ12345678901234", "XABC12", 100m));
        var command = ValidCommand();
        command.RegistrationNumber = "ABC";

        await CreateService(store).Create(command);

        Assert.Equal(2, store.Data.Count);
    }

    [Fact]
    public async Task Update_WhenVehicleMissing_ThenReportsNotFound()
    {
        await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
            CreateService(new VehicleStoreFake()).Update(ValidCommand(), Guid.NewGuid()));
    }

    [Fact]
    public async Task Get_WhenVehicleMissing_ThenReportsNotFound()
    {
        await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
            CreateService(new VehicleStoreFake()).Get(Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task Search_WhenFiltersAndSortProvided_ThenForwardsCriteriaAndReturnsTotal()
    {
        var store = new VehicleStoreFake();
        store.Data.Add(Vehicle.Rehydrate(Guid.NewGuid(), "Alex Smith", MazdaId, "Mazda", 2020,
            "ABC12345678901234", "ABC123", 1850.75m));
        var result = await CreateService(store).Search(new VehicleSearchQuery
        {
            OwnerName = "Alex", ManufacturerName = "Mazda", Year = 2020,
            MinWeightKg = 500m, MaxWeightKg = 2500m, PageNumber = 2,
            SortFieldList = [new FieldSortOrder { FieldName = "weight", IsAscending = false }]
        }, CancellationToken.None);
        Assert.Equal("Alex", store.LastOwnerName);
        Assert.Equal("Mazda", store.LastManufacturer);
        Assert.Equal(2, store.LastPageNumber);
        Assert.Equal("weight", Assert.Single(store.LastSortingList!).FieldName);
        Assert.False(store.LastSortingList![0].IsAscending);
        Assert.Equal(500m, store.LastMinWeightKg);
        Assert.Equal(2500m, store.LastMaxWeightKg);
        Assert.Equal(1, result.TotalRecords);
        Assert.Equal(2, result.PageNumber);
        Assert.Empty(result.Data);
    }

    private sealed class VehicleStoreFake : IVehicleStore
    {
        public List<Vehicle> Data { get; } = [];
        public string? LastOwnerName { get; private set; }
        public string? LastManufacturer { get; private set; }
        public int LastPageNumber { get; private set; }
        public List<FieldSortOrder>? LastSortingList { get; private set; }
        public decimal? LastMinWeightKg { get; private set; }
        public decimal? LastMaxWeightKg { get; private set; }

        public Task<Vehicle?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Data.SingleOrDefault(x => x.Id == id));

        public Task<List<Vehicle>> QueryAsync(string? ownerName = null, string? manufacturer = null,
            int? year = null, string? vin = null, string? registrationNumber = null,
            decimal? minWeightKg = null, decimal? maxWeight = null, int pageNumber = 1, List<FieldSortOrder>? sortingList = null,
            CancellationToken cancellationToken = default)
        {
            LastOwnerName = ownerName;
            LastManufacturer = manufacturer;
            LastPageNumber = pageNumber;
            LastSortingList = sortingList;
            LastMinWeightKg = minWeightKg;
            LastMaxWeightKg = maxWeight;
            return Task.FromResult(Data.Where(x =>
                (ownerName is null || x.OwnerName.Contains(ownerName)) &&
                (manufacturer is null || x.ManufacturerName?.Contains(manufacturer) == true) &&
                (year is null || x.Year == year) &&
                (vin is null || x.Vin == vin) &&
                (registrationNumber is null || x.RegistrationNumber == registrationNumber) &&
                (minWeightKg is null || x.WeightKg >= minWeightKg) &&
                (maxWeight is null || x.WeightKg < maxWeight))
                .Skip(10 * (pageNumber - 1)).Take(10).ToList());
        }

        public Task AddAsync(Vehicle vehicle)
        {
            Data.Add(vehicle);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Vehicle vehicle) => Task.CompletedTask;
        public Task<int> QueryCountAsync(string? ownerName = null, string? manufacturer = null,
            int? year = null, string? vin = null, string? registrationNumber = null,
            decimal? minWeightKg = null, decimal? maxWeightKg = null,
            CancellationToken cancellationToken = default) => Task.FromResult(Data.Count(x =>
                (ownerName is null || x.OwnerName.Contains(ownerName)) &&
                (manufacturer is null || x.ManufacturerName?.Contains(manufacturer) == true) &&
                (year is null || x.Year == year) &&
                (minWeightKg is null || x.WeightKg >= minWeightKg) &&
                (maxWeightKg is null || x.WeightKg < maxWeightKg)));
        public Task DeleteAsync(Guid id) => Task.CompletedTask;
    }

    private sealed class ManufacturerStoreFake : IManufacturerStore
    {
        private readonly Manufacturer _mazda = new(MazdaId, "Mazda");
        public Task<Manufacturer?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<Manufacturer?>(id == MazdaId ? _mazda : null);
        public Task<Manufacturer?> GetByNameAsync(string name, CancellationToken cancellationToken = default) =>
            Task.FromResult<Manufacturer?>(name == "Mazda" ? _mazda : null);
        public Task<List<Manufacturer>> GetAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(new List<Manufacturer> { _mazda });
    }

    private sealed class LoggerFake : ILoggingService<VehicleService>
    {
        public void LogDebug(string messageTemplate, params object?[] args) { }
        public void LogWarning(Exception ex, string messageTemplate, params object?[] args) { }
        public void LogInfo(string messageTemplate, params object?[] args) { }
        public void LogError(Exception ex, string messageTemplate, params object?[] args) { }
    }
}
