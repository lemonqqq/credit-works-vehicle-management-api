using CreditWorks.VehicleManagement.Application.Services;
using CreditWorks.VehicleManagement.Domain.Commands;
using CreditWorks.VehicleManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CreditWorks.VehicleManagement.Controllers
{
    [ApiController]
    [Route("api/vehicles")]
    public class VehiclesApiController(VehicleWriteService vehicleWriteService) : ControllerBase
    {
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return Ok();
        }

        [HttpGet("list")]
        public async Task<IActionResult> List()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] VehicleCommand command, CancellationToken cancellationToken)
        {
            var result = await vehicleWriteService.CreateAsync(command, cancellationToken);
            if (result.Error != VehicleWriteError.None) return Failure(result);

            var vehicle = result.Vehicle!;
            return Created($"/api/vehicles/{vehicle.Id}", ToResponse(vehicle));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] VehicleCommand command, CancellationToken cancellationToken)
        {
            var result = await vehicleWriteService.UpdateAsync(id, command, cancellationToken);
            return result.Error == VehicleWriteError.None ? Ok(ToResponse(result.Vehicle!)) : Failure(result);
        }

        private IActionResult Failure(VehicleWriteResult result) => result.Error switch
        {
            VehicleWriteError.NotFound => NotFound(new { error = result.Message }),
            VehicleWriteError.DuplicateVin or VehicleWriteError.DuplicateRegistrationNumber =>
                Conflict(new { error = result.Message }),
            _ => BadRequest(new { error = result.Message })
        };

        private static object ToResponse(Vehicle vehicle) => new
        {
            vehicle.Id,
            vehicle.OwnerName,
            vehicle.ManufacturerId,
            vehicle.Year,
            vehicle.Vin,
            vehicle.RegistrationNumber,
            vehicle.WeightKg
        };
    }
}
