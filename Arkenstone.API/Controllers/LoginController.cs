using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using ESI.NET;
using System.Threading.Tasks;
using ESI.NET.Models.SSO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Arkenstone.Entities;
using System.Linq;
using Arkenstone.Logic.Esi;

namespace Arkenstone.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : Controller
    {

        private readonly ILogger<LoginController> _logger;
        private readonly ArkenstoneContext _context;

        public LoginController(ArkenstoneContext context, ILogger<LoginController> logger)
        {
            _logger = logger;
            _context = context;
        }

        private string CreateToken(AuthorizedCharacterData data, string code)
        {
            var token = new JwtTokenBuilder()
                                .AddSecurityKey(JwtSecurityKey.Create(System.Environment.GetEnvironmentVariable("TokenSecretKey")))
                                .AddSubject(data.CharacterName)
                                .AddIssuer(System.Environment.GetEnvironmentVariable("TokenIssuer"))
                                .AddAudience(System.Environment.GetEnvironmentVariable("TokenIssuer"))
                                .AddClaim("CCPId", data.CharacterID.ToString())
                                .AddClaim("CCPToken", code)
                                .AddExpiry(4320)
                                .Build();

            return token.Value;
        }

        [HttpGet("geturllogin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public IActionResult geturllogin()
        {
            EveEsi_Connexion connexion = new EveEsi_Connexion();
            return Ok(connexion.GetUrlConnection());
        }

        [HttpGet("callbackccp")]
        public async Task<IActionResult> Login(string code)
        {
            EveEsi_Connexion connexion = new EveEsi_Connexion();
            await connexion.GetToken(code);
            await connexion.ConnectCharCCP();

            var idCorp = System.Environment.GetEnvironmentVariable("CorporationId");
            if (connexion.authorizedCharacterData.CorporationID.ToString() == idCorp)
            {
                
                var character = _context.Characters.Find(connexion.authorizedCharacterData.CharacterID);
                if (character == null)
                {
                    character = new Entities.DbSet.Character() { Id = connexion.authorizedCharacterData.CharacterID, Name = connexion.authorizedCharacterData.CharacterName };
                    _context.Characters.Add(character);
                }
                character.RefreshToken = connexion.ssoToken.RefreshToken;
                character.Token = connexion.ssoToken.AccessToken;
                _context.SaveChanges();

                return Ok(this.CreateToken(connexion.authorizedCharacterData, code));
            }
            else
                return Unauthorized();
        }
    }
}
