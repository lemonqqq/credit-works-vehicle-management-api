using CreditWorks.VehicleManagement.Application.Contracts;
using CreditWorks.VehicleManagement.Application.Mappers;
using CreditWorks.VehicleManagement.Application.Validators;
using CreditWorks.VehicleManagement.Domain.Aggregates;
using CreditWorks.VehicleManagement.Domain.Commands;
using CreditWorks.VehicleManagement.Domain.Exceptions;
using CreditWorks.VehicleManagement.Domain.Queries;
using CreditWorks.VehicleManagement.Domain.Queries.Dto;

namespace CreditWorks.VehicleManagement.Application.Services.Impl
{
    public sealed class VehicleService(IVehicleStore vehicleStore, IManufacturerStore manufacturerStore,
        ILoggingService<VehicleService> logger)
        : IVehicleService
    {

        public async Task<Vehicle> Validate(VehicleCommand command, Guid? id = null)
        {
            var manufacturer = await manufacturerStore.GetByNameAsync(command.Manufacturer)
                ?? throw new ResourceNotFoundException($"Manufacturer {command.Manufacturer} was not found");

            Vehicle vehicle;
            if (id.HasValue)
            {
                vehicle = await vehicleStore.GetAsync(id.Value)
                    ?? throw new ResourceNotFoundException($"Vehicle {id} was not found");
                if (!string.Equals(vehicle.ManufacturerName, command.Manufacturer, StringComparison.Ordinal))
                    logger.LogInfo($"Switching manufacturer from {vehicle.ManufacturerName} to {command.Manufacturer}");
                vehicle.Update(command.OwnerName, manufacturer.Id, command.Year, command.Vin,
                    command.RegistrationNumber, command.WeightKg, manufacturer.Name);
            }
            else
                vehicle = Vehicle.Create(command.OwnerName, manufacturer.Id, command.Year,
                    command.Vin, command.RegistrationNumber, command.WeightKg);
            return vehicle;
        }

        public async Task Create(Vehicle vehicle)
        {
            logger.LogInfo("Create - start", vehicle);
            VehicleValidator.ValidateVin(vehicle, await vehicleStore.QueryAsync(vin: vehicle.Vin));
            VehicleValidator.ValidateRegistrationNumber(vehicle, await vehicleStore.QueryAsync(registrationNumber: vehicle.RegistrationNumber));

            await vehicleStore.AddAsync(vehicle);
            logger.LogInfo("Create - end", vehicle);
        }

        public async Task Update(Vehicle vehicle)
        {
            logger.LogInfo("Update - start", vehicle);

            VehicleValidator.ValidateVin(vehicle, await vehicleStore.QueryAsync(vin: vehicle.Vin));
            VehicleValidator.ValidateRegistrationNumber(vehicle, await vehicleStore.QueryAsync(registrationNumber: vehicle.RegistrationNumber));
            await vehicleStore.UpdateAsync(vehicle);

            logger.LogInfo("Update - end", vehicle);
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

        public async Task<IEnumerable<VehicleDto>> Search(VehicleSearchQuery query,
            List<FieldSortOrder> orderFields = null, CancellationToken cancellationToken = default)
        {
            logger.LogInfo("Search - start", query);
            var strSortingList = string.Join(", ", orderFields.Select(x => $"{x.FieldName} {(x.IsAscending ? "asc" : "desc")}"));

            var vehicles = await vehicleStore.QueryAsync(query.OwnerName, query.ManufacturerName,
                query.Year, query.Vin, query.RegistrationNumber, query.MinWeightKg, query.MaxWeightKg, 
                query.PageNumber, strSortingList, cancellationToken);

            if (vehicles == null || vehicles.Count == 0)
                return [];

            var result = vehicles.Select(VehicleMapper.ToDto);

            logger.LogInfo("Search - end", query);
            return result;
        }
    }
}
