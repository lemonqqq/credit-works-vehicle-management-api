using Microsoft.AspNetCore.Mvc;

namespace CreditWorks.VehicleManagement.REST
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CategoriesApiController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
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

        [HttpDelete("/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok();
        }
    }
}
