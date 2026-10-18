using CreditWorks.VehicleManagement.Application.Contracts;
using CreditWorks.VehicleManagement.Application.Services.Impl;
using CreditWorks.VehicleManagement.Domain.Aggregates;
using CreditWorks.VehicleManagement.Domain.Commands;
using CreditWorks.VehicleManagement.Domain.Queries;

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

        await Assert.ThrowsAsync<CreditWorks.VehicleManagement.Domain.Exceptions.BusinessValidationException>(
            () => CreateService(store).Validate(command));
        Assert.Empty(store.Data);
    }

    [Fact]
    public async Task Create_WhenValidated_ThenAddsVehicle()
    {
        var store = new VehicleStoreFake();
        var service = CreateService(store);
        var vehicle = await service.Validate(ValidCommand());
        await service.Create(vehicle);
        Assert.Equal(vehicle.Id, Assert.Single(store.Data).Id);
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
        var validated = await service.Validate(command, existing.Id);
        await service.Update(validated);

        Assert.Equal("Taylor Smith", store.Data[0].OwnerName);
        Assert.Equal(499.99m, store.Data[0].WeightKg);
    }

    [Fact]
    public async Task Search_WhenNoFiltersProvided_ThenReturnsAllVehicles()
    {
        var store = new VehicleStoreFake();
        store.Data.Add(Vehicle.Rehydrate(Guid.NewGuid(), "Alex Smith", MazdaId, "Mazda", 2020,
            "ABC12345678901234", "ABC123", 1850.75m));

        var result = (await CreateService(store).Search(new VehicleSearchQuery(), [], CancellationToken.None)).ToList();
        Assert.Single(result);
        Assert.Equal("Mazda", result[0].Manufacturer);
    }

    [Fact]
    public async Task Search_WhenNoVehiclesMatch_ThenReturnsEmptyList()
    {
        var result = await CreateService(new VehicleStoreFake())
            .Search(new VehicleSearchQuery(), [], CancellationToken.None);
        Assert.Empty(result);
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

    private sealed class VehicleStoreFake : IVehicleStore
    {
        public List<Vehicle> Data { get; } = [];

        public Task<Vehicle?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Data.SingleOrDefault(x => x.Id == id));

        public Task<List<Vehicle>> QueryAsync(string? ownerName = null, string? manufacturer = null,
            int? year = null, string? vin = null, string? registrationNumber = null,
            decimal? minWeightKg = null, decimal? maxWeight = null, int pageNumber = 1, string? sortingList = null,
            CancellationToken cancellationToken = default) => Task.FromResult(Data.Where(x =>
                (ownerName is null || x.OwnerName.Contains(ownerName)) &&
                (manufacturer is null || x.ManufacturerName.Contains(manufacturer) == true) &&
                (year is null || x.Year == year) &&
                (vin is null || x.Vin.Contains(vin)) &&
                (registrationNumber is null || x.RegistrationNumber.Contains(registrationNumber)) &&
                (minWeightKg is null || x.WeightKg >= minWeightKg) &&
                (maxWeight is null || x.WeightKg <= maxWeight)).ToList());

        public Task AddAsync(Vehicle vehicle)
        {
            Data.Add(vehicle);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Vehicle vehicle) => Task.CompletedTask;
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
