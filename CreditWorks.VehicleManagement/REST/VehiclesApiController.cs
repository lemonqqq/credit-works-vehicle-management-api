using Microsoft.AspNetCore.Mvc;

namespace CreditWorks.VehicleManagement.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class VehiclesApiController : ControllerBase
    {
        [HttpGet("/{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return Ok();
        }

        [HttpGet("/list")]
        public async Task<IActionResult> List()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            return Ok();
        }

        [HttpPut("/{id}")]
        public async Task<IActionResult> Put(Guid id)
        {
            return Ok();
        }
    }
}
