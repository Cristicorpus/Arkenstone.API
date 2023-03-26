using Arkenstone.API.Controllers;
using Arkenstone.API.Services;
using Arkenstone.Logic.Esi;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Arkenstone.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : OriginController
    {
        public LoginController(ArkenstoneContext context) : base(context)
        {

        }

        [HttpGet("geturllogin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public IActionResult geturllogin()
        {
            Character tokenCharacter = null;
            try
            {
                tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            }
            catch (Exception )
            {
                tokenCharacter = null;
            }

            var _eveEsiConnexion = new EveEsiConnexion();
            string response;

            if (tokenCharacter == null)
                response = _eveEsiConnexion.GetUrlConnection();
            else
                response = _eveEsiConnexion.GetUrlConnection(tokenCharacter.CharacterMainId.ToString());

            if (System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "FeoDev")
                return Ok(response);
            else
                return Redirect(response);
        }

        [HttpGet("callbackccp")]
        public async Task<IActionResult> Login([FromQuery] string code, [FromQuery] string state = null)
        {
            if (code == "")
                return Redirect(Environment.GetEnvironmentVariable("FrontCallBack") + "?error=" + "401");

            var _eveEsiConnexion = new EveEsiConnexion();
            try
            {
                await _eveEsiConnexion.GetToken(code);
                await _eveEsiConnexion.ConnectCharCCP();
            }
            catch (System.Exception)
            {
                return Redirect(Environment.GetEnvironmentVariable("FrontCallBack") + "?error=" + "401");
            }

            var characterService = new CharacterService(_context);
            var corporationService = new CorporationService(_context);
            var allianceService = new AllianceService(_context);

            allianceService.GetOrCreate(_eveEsiConnexion.authorizedCharacterData.AllianceID);
            corporationService.GetOrCreate(_eveEsiConnexion.authorizedCharacterData.CorporationID);


            var characterAuthorized = characterService.GetAndUpdateByauthorizedCharacterData(_eveEsiConnexion.authorizedCharacterData, _eveEsiConnexion.ssoToken);

            //ici on met a jour le mainid
            int mainCharacterId = characterAuthorized.Id;
            if (state != null && int.TryParse(state, out mainCharacterId))
            {
                if (mainCharacterId>0 && characterService.Get(mainCharacterId)!=null)
                    characterAuthorized = characterService.CharacterSetMain(characterAuthorized.Id, mainCharacterId);
            }

            string url = Environment.GetEnvironmentVariable("FrontCallBack") + "?token=" + TokenService.Createtoken(characterAuthorized);

            return Redirect(url);
        }
    }
}
