using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Arkenstone.Controllers
{
    [Route("")]
    [ApiController]
    public class HealthController
    {
        private readonly ILogger<RecipeController> _logger;

        public HealthController(ILogger<RecipeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return "ok";
        }
    }
}
