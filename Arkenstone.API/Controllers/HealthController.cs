using Arkenstone.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace Arkenstone.API.Controllers
{
    [Route("")]
    [ApiController]
    public class HealthController : OriginController
    {
        public HealthController(ArkenstoneContext context) : base(context)
        {

        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
