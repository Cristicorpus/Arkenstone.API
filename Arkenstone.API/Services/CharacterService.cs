using ESI.NET.Models.SSO;
using Arkenstone.API.Models;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using EveMiningFleet.API.Models;
using Arkenstone.Logic.BusinessException;
using System.Xml.Linq;

namespace Arkenstone.API.Services
{
    public class CharacterService
    {
        private ArkenstoneContext _context;

        public CharacterService(ArkenstoneContext context)
        {
            _context = context;
        }
        private IQueryable<Character> GetCore()
        {
            return _context.Characters.Include("Corporation").Include("Alliance").Include("CharacterMain");
        }

        public Character GetAndUpdateByauthorizedCharacterData(AuthorizedCharacterData authorizedCharacterData, SsoToken accessToken)
        {
            Character characterConnexion = GetCore().FirstOrDefault((x) => x.Id == authorizedCharacterData.CharacterID);
            if (characterConnexion == null)
            {
                characterConnexion = new Character();

                characterConnexion.Id = authorizedCharacterData.CharacterID;
                characterConnexion.CharacterMainId = authorizedCharacterData.CharacterID;
                _context.Characters.Add(characterConnexion);
            }
            characterConnexion.AllianceId = authorizedCharacterData.AllianceID;
            characterConnexion.CorporationId = authorizedCharacterData.CorporationID;
            //on met a jours les informations du player.
            characterConnexion.Name = authorizedCharacterData.CharacterName;
            characterConnexion.Token = accessToken.AccessToken;
            characterConnexion.RefreshToken = accessToken.RefreshToken;
            _context.SaveChanges();
            return characterConnexion;
        }

        public Character Get(int id)
        {
            Entities.DbSet.Character character = GetCore().FirstOrDefault(x => x.Id == id);
            if (character == null)
                throw new NotFound("Character");
            else
                return character;
        }
        public Character GetByName(string name)
        {
            Entities.DbSet.Character character = GetCore().FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
            if (character == null)
                throw new NotFound("Character");
            else
                return character;
        }
        public List<Character> GetListFromMain(int mainId)
        {
            return GetCore().Where(x => x.Id != mainId && x.CharacterMainId == mainId).ToList();
        }


        

        public async void UpdateMainId(int oldMainId, int mainId)
        {
            var characters = GetCore().Where(x => x.CharacterMainId == oldMainId);

            await characters.ForEachAsync(x => x.CharacterMainId = mainId);
            _context.SaveChanges();
        }

        public Character SetMain(int id, int mainID)
        {
            Character character = _context.Characters.FirstOrDefault(x => x.Id == id);
            if (character != null)
            {
                character.CharacterMainId = mainID;
                _context.SaveChanges();
            }
            return character;
        }   
    }
}
