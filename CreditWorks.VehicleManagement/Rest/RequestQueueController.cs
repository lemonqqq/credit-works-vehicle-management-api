using CreditWorks.VehicleManagement.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CreditWorks.VehicleManagement.Rest
{
    [ApiController]
    [Route("/api/request")]
    public class RequestQueueController(IRequestQueueWriteService requestQueueService) : Controller
    {
        private static readonly string[] queueStateToRemove = [ "succeeded", "failed" ];

        [HttpGet("{id:guid}")]
        public IActionResult Get(Guid id)
        {
            var status = requestQueueService.Get(id);
            // remove request from the queue if it is succeeded
            if (queueStateToRemove.Contains(status?.State))
            {
                requestQueueService.Remove(id);
            }
            return status == null ? NotFound() : Ok(status);
        }
    }
}
