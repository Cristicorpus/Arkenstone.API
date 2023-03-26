using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using ESI.NET.Models.SSO;
using System.Collections.Generic;

namespace Arkenstone.API.Services
{
    public class CharacterService : BaseService
    {
        public CharacterService(ArkenstoneContext context) : base(context)
        {

        }

        public Character GetAndUpdateByauthorizedCharacterData(AuthorizedCharacterData authorizedCharacterData, SsoToken accessToken)
        {

            return characterRepository.GetAndUpdate(authorizedCharacterData.CharacterID, 
                authorizedCharacterData.CorporationID, 
                authorizedCharacterData.AllianceID, 
                authorizedCharacterData.CharacterName, 
                accessToken.AccessToken, 
                accessToken.RefreshToken);
        }
        public Character Get(int id)
        {
            return characterRepository.Get(id);
        }
        public Character GetByName(string name)
        {
            return characterRepository.GetByName(name);
        }
        public List<Character> GetListFromMain(int mainId)
        {
            return characterRepository.GetListFromMain(mainId);
        }

        public Character CharacterSetMain(int id, int mainID)
        {
            return characterRepository.CharacterSetMain(id, mainID);
        }
        public async void BulkUpdateMainId(int oldMainId, int mainId)
        {
            characterRepository.BulkUpdateMainId(oldMainId, mainId);
        }
    }
}
