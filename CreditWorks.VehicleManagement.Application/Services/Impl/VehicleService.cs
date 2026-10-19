using CreditWorks.VehicleManagement.Application.Contracts;
using CreditWorks.VehicleManagement.Application.Mappers;
using CreditWorks.VehicleManagement.Application.Models.Commands;
using CreditWorks.VehicleManagement.Application.Models.Queries;
using CreditWorks.VehicleManagement.Application.Models.Queries.Dto;
using CreditWorks.VehicleManagement.Application.Validators;
using CreditWorks.VehicleManagement.Domain.Aggregates;
using CreditWorks.VehicleManagement.Domain.Exceptions;

namespace CreditWorks.VehicleManagement.Application.Services.Impl
{
    public sealed class VehicleService(IVehicleStore vehicleStore, IManufacturerStore manufacturerStore,
        ILoggingService<VehicleService> logger)
        : IVehicleService
    {

        private async Task<Vehicle> Validate(VehicleCommand command, Guid? id = null)
        {
            var manufacturer = await manufacturerStore.GetByNameAsync(command.Manufacturer)
                ?? throw new ResourceNotFoundException($"Manufacturer {command.Manufacturer} was not found");

            if (id.HasValue)
            {
                var vehicle = await vehicleStore.GetAsync(id.Value)
                    ?? throw new ResourceNotFoundException($"Vehicle {id} was not found");
                if (!string.Equals(vehicle.ManufacturerName, command.Manufacturer, StringComparison.Ordinal))
                    logger.LogInfo($"Switching manufacturer from {vehicle.ManufacturerName} to {command.Manufacturer}");
                vehicle.Update(command.OwnerName, manufacturer.Id, command.Year, command.Vin,
                    command.RegistrationNumber, command.WeightKg, manufacturer.Name);
                return vehicle;
            }
            return Vehicle.Create(command.OwnerName, manufacturer.Id, command.Year,
                    command.Vin, command.RegistrationNumber, command.WeightKg);
        }

        public async Task Create(VehicleCommand command)
        {
            logger.LogInfo("Create - start", command);

            var vehicle = await Validate(command);
            VehicleValidator.ValidateVin(vehicle, await vehicleStore.QueryAsync(vin: vehicle.Vin));
            VehicleValidator.ValidateRegistrationNumber(vehicle, await vehicleStore.QueryAsync(registrationNumber: vehicle.RegistrationNumber));
            await vehicleStore.AddAsync(vehicle);

            logger.LogInfo("Create - end", command);
        }

        public async Task Update(VehicleCommand command, Guid id)
        {
            logger.LogInfo("Update - start", command);

            var vehicle = await Validate(command, id);
            VehicleValidator.ValidateVin(vehicle, await vehicleStore.QueryAsync(vin: vehicle.Vin));
            VehicleValidator.ValidateRegistrationNumber(vehicle, await vehicleStore.QueryAsync(registrationNumber: vehicle.RegistrationNumber));
            await vehicleStore.UpdateAsync(vehicle);

            logger.LogInfo("Update - end", command);
        }

        public async Task<VehicleDto> Get(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInfo("Get - start", id);
            var vehicle = await vehicleStore.GetAsync(id, cancellationToken)
                ?? throw new ResourceNotFoundException($"Vehicle {id} was not found.");

            var result = VehicleMapper.ToDto(vehicle);

            logger.LogInfo("Get - end", id);
            return result;
        }

        public async Task<VehicleSearchQueryResult> Search(VehicleSearchQuery query, CancellationToken cancellationToken = default)
        {
            logger.LogInfo("Search - start", query);

            var vehicles = await vehicleStore.QueryAsync(query.OwnerName, query.ManufacturerName,
                query.Year, minWeightKg: query.MinWeightKg, maxWeightKg: query.MaxWeightKg, 
                pageNumber: query.PageNumber, sortingList: query.SortFieldList, cancellationToken: cancellationToken);

            if (vehicles == null || vehicles.Count == 0)
                return new([], 0, 0);

            var result = vehicles.Select(VehicleMapper.ToDto).ToList();
            var noOfRecords = await vehicleStore.QueryCountAsync(query.OwnerName, query.ManufacturerName,
                query.Year, minWeightKg: query.MinWeightKg, maxWeightKg: query.MaxWeightKg,
                cancellationToken: cancellationToken);

            logger.LogInfo("Search - end", query);
            return new VehicleSearchQueryResult(result, query.PageNumber, noOfRecords);
        }
    }
}
