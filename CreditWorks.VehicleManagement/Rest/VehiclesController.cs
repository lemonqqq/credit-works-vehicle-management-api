using CreditWorks.VehicleManagement.Application.Contracts;
using CreditWorks.VehicleManagement.Application.Models.Commands;
using CreditWorks.VehicleManagement.Application.Models.Queries;
using CreditWorks.VehicleManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CreditWorks.VehicleManagement.Rest
{
    /// <summary>
    ///     Exposes vehicle read, search, create, and update endpoints.
    /// </summary>
    /// <param name="vehicleService">The vehicle application service.</param>
    /// <param name="requestQueueService">The shared background-work queue.</param>
    /// <param name="logger">The controller logger.</param>
    [ApiController]
    [Route("api/vehicles")]
    public class VehiclesController(IVehicleService vehicleService,
        IRequestQueueWriteService requestQueueService,
        ILoggingService<VehiclesController> logger) 
        : BaseController<VehiclesController>(logger)
    {

        /// <summary>
        ///     Gets a vehicle by its identifier.
        /// </summary>
        /// <param name="id">The vehicle identifier.</param>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <returns>
        ///     The vehicle response or an error response.
        /// </returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken = default)
            => await ToResponse(() => vehicleService.Get(id, cancellationToken));

        /// <summary>
        ///     Searches vehicles using the supplied criteria.
        /// </summary>
        /// <param name="query">The vehicle search criteria.</param>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <returns>
        ///     The matching vehicles or an error response.
        /// </returns>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] VehicleSearchQuery query, CancellationToken cancellationToken = default)
            => await ToResponse(() => vehicleService.Search(query, cancellationToken));

        /// <summary>
        ///     Creates a vehicle.
        /// </summary>
        /// <param name="command">The vehicle details to create.</param>
        /// <returns>
        ///     An accepted queue identifier or an error response.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] VehicleCommand command)
            => await ToResponse(async() =>
            {
                if (!requestQueueService.TryEnqueue(
                    services => services.GetRequiredService<IVehicleService>().Create(command), out var queueId))
                    return Problem(statusCode: 503, title: "Queue is full");

                return Accepted(queueId);
            });

        /// <summary>
        ///     Updates an existing vehicle.
        /// </summary>
        /// <param name="id">The vehicle identifier.</param>
        /// <param name="command">The replacement vehicle details.</param>
        /// <returns>
        ///     An accepted queue identifier or an error response.
        /// </returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] VehicleCommand command)
            => await ToResponse(async () =>
            {
                if (!requestQueueService.TryEnqueue(
                    services => services.GetRequiredService<IVehicleService>().Update(command, id), out var queueId))
                    return Problem(statusCode: 503, title: "Queue is full");

                return Accepted(queueId);
            });
    }
}
