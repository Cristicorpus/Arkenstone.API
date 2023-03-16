using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
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
            if(httpContext==null) return null;
            var httpcontextuserIdentity = httpContext.User.Identity;

            var identity = httpcontextuserIdentity as System.Security.Claims.ClaimsIdentity;

            var claimObject = identity?.FindFirst(TokenService.characterIdClaimKey);
            if (claimObject == null) return null;

            int tokenCharacterId;
            if (!int.TryParse(claimObject.Value, out tokenCharacterId)) return null;
            
            Character response = DbContext.Characters.FirstOrDefault(x => x.Id == tokenCharacterId);
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
                    .AddClaim(TokenService.characterCorpClaimKey, character.AllianceId.ToString())
                    .AddExpiry(4320)
                    .Build();

            return token.Value;
        }
    }

    public class TokenServiceCharacters{
        public Character main { get; set; }
        public List<Character> all { get; set; } = new List<Character>();

        public TokenServiceCharacters(Character character)
                {
                    main = character;
                    all.Add(character);
                }
        public void addCharacter(Character character)
        {
            if (!all.Any(x => x.Id == character.Id))
                all.Add(character);
        }
    }

}
