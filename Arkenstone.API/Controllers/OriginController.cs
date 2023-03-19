using Arkenstone.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Arkenstone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class OriginController : Controller
    {
        protected readonly ArkenstoneContext _context;
        public OriginController(ArkenstoneContext context)
        {
            _context = context;
        }

    }
}
