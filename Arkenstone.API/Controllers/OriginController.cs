using Arkenstone.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Arkenstone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class OriginController : ControllerBase
    {
        protected readonly ArkenstoneContext _context;
        public OriginController(ArkenstoneContext context)
        {
            _context = context;
        }

    }
}
