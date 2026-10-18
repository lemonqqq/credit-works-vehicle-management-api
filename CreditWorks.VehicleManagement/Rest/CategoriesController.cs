using CreditWorks.VehicleManagement.Application.Contracts;
using CreditWorks.VehicleManagement.Application.Services;
using CreditWorks.VehicleManagement.Domain.Commands;
using CreditWorks.VehicleManagement.Infrastructure.Common;
using Microsoft.AspNetCore.Mvc;

namespace CreditWorks.VehicleManagement.Rest
{
    /// <summary>
    ///     Exposes category listing and configuration endpoints.
    /// </summary>
    /// <param name="categoryService">The category application service.</param>
    /// <param name="requestQueueService">The shared background-work queue.</param>
    /// <param name="logger">The controller logger.</param>
    [ApiController]
    [Route("/api/categories")]
    public class CategoriesController(IVehicleCategoryService categoryService,
        RequestQueueService requestQueueService,
        ILoggingService<CategoriesController> logger) : BaseController<CategoriesController>(logger)
    {
        /// <summary>
        ///     Gets the configured vehicle categories.
        /// </summary>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <returns>
        ///     The categories or an error response.
        /// </returns>
        [HttpGet("list")]
        public async Task<IActionResult> List(CancellationToken cancellationToken)
            => await ToResponse(() => categoryService.GetAll(cancellationToken));

        /// <summary>
        ///     Applies a proposed category configuration.
        /// </summary>
        /// <param name="command">The complete category set to maintain.</param>
        /// <returns>
        ///     An accepted queue identifier or an error response.
        /// </returns>
        [HttpPatch]
        public async Task<IActionResult> Patch(CategoryCommand command)
            => await ToResponse(async() =>
            {
                categoryService.Validate(command.Categories);
                if (!requestQueueService.TryEnqueue(async services =>
                    await services.GetRequiredService<IVehicleCategoryService>()
                        .Maintain(command, CancellationToken.None), out var queueId))
                    return Problem(statusCode: 503, title: "Queue is full");
                return Accepted(queueId);
            });
    }
}
