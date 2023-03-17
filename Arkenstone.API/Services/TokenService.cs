using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.BusinessException;
using Arkenstone.Logic.Token.jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arkenstone.API.Services
{
    public static class TokenService
    {
        public static readonly string characterIdClaimKey = "CharacterIdClaimKey";
        public static readonly string characterNameClaimKey = "CharacterNameClaimKey";
        public static readonly string characterCorpClaimKey = "CharacterCorpClaimKey";
        public static readonly string characterAllianceClaimKey = "CharacterAllianceClaimKey";
        public static Character GetCharacterFromToken(ArkenstoneContext DbContext, HttpContext httpContext)
        {
            if(httpContext==null) throw new NotAuthentified();
            var httpcontextuserIdentity = httpContext.User.Identity;

            var identity = httpcontextuserIdentity as System.Security.Claims.ClaimsIdentity;

            var claimObject = identity?.FindFirst(TokenService.characterIdClaimKey);
            if (claimObject == null) throw new NotAuthentified();

            int tokenCharacterId;
            if (!int.TryParse(claimObject.Value, out tokenCharacterId)) return null;
            
            Character response = DbContext.Characters.FirstOrDefault(x => x.Id == tokenCharacterId);
            if (response == null) throw new NotAuthentified();

            return response;
        }

        
        public static string Createtoken(Character character)
        {
            var tokenKey = Encoding.UTF8.GetBytes(System.Environment.GetEnvironmentVariable("TokenSecretKey"));
            var token = new JwtTokenBuilder()
                    .AddSecurityKey(new SymmetricSecurityKey(tokenKey))
                    .AddIssuer(System.Environment.GetEnvironmentVariable("TokenIssuer"))
                    .AddAudience(System.Environment.GetEnvironmentVariable("TokenIssuer"))
                    .AddClaim(TokenService.characterIdClaimKey, character.CharacterMainId.ToString())
                    .AddClaim(TokenService.characterNameClaimKey, character.Name.ToString())
                    .AddClaim(TokenService.characterCorpClaimKey, character.CorporationId.ToString())
                    .AddClaim(TokenService.characterAllianceClaimKey, character.AllianceId.ToString())
                    .AddExpiry(4320)
                    .Build();

            return token.Value;
        }
    }


}
