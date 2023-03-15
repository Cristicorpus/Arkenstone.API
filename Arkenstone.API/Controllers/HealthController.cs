using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;



namespace Arkenstone.Controllers
{
    [Route("")]
    [ApiController]
    public class HealthController : Controller
    {
        private readonly ILogger<RecipeController> _logger;

        public HealthController(ILogger<RecipeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
