using ESI.NET.Models.SSO;
using Arkenstone.API.Models;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using EveMiningFleet.API.Models;

namespace Arkenstone.API.Services
{
    public class CharacterService
    {
        private ArkenstoneContext _context;

        public CharacterService(ArkenstoneContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Create and update character by esi login.
        /// </summary>
        /// <param name="authorizedCharacterData"></param>
        /// <param name="accessToken"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public Character GetAndUpdateByauthorizedCharacterData(AuthorizedCharacterData authorizedCharacterData, SsoToken accessToken)
        {
            Character characterConnexion = _context.Characters.FirstOrDefault((x) => x.Id == authorizedCharacterData.CharacterID);
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

        /// <summary>
        /// Get a character by his id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CharacterModel Get(int id)
        {
            Entities.DbSet.Character character = _context.Characters.Include("Corporation").Include("Alliance").FirstOrDefault(x => x.Id == id);
            if (character == null)
                return null;
            else
                return new CharacterModel(character);
        }
        /// <summary>
        /// Get all characters with the same main id.
        /// </summary>
        /// <param name="mainId"></param>
        /// <returns></returns>
        public List<CharacterModel> GetByMainId(int mainId)
        {
            return _context.Characters.Include("Corporation").Include("Alliance").Where(x => x.CharacterMainId == mainId).Select(_Character => new CharacterModel(_Character)).ToList();
        }
        
        /// <summary>        
        /// Get all characters.
        /// </summary>
        /// <returns></returns>
        public List<CharacterModel> GetAll()
        {
            return _context.Characters.Include("Corporation").Include("Alliance").Select(_Character => new CharacterModel(_Character)).ToList();
        }
        /// <summary>
        /// Get a character by his name. The name is not case sensitive and the space are not important.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<CharacterModel> GetByName(string name)
        {
            return _context.Characters.Include("Corporation").Include("Alliance").Where(x => x.Name.ToLower().Replace(" ", "") == name.ToLower().Replace(" ", "")).Select(_Character => new CharacterModel(_Character)).ToList();
        }
        
        /// <summary>
        /// Update the main id of all characters with the old main id to the new main id.
        /// </summary>
        /// <param name="oldMainId"></param>
        /// <param name="mainId"></param>
        public async void UpdateMainId(int oldMainId, int mainId)
        {
            var characters = _context.Characters.Include("Corporation").Include("Alliance").Where(x => x.CharacterMainId == oldMainId);

            await characters.ForEachAsync(x => x.CharacterMainId = mainId);
            _context.SaveChanges();
        }
        /// <summary>
        /// Set the main id of a character.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mainID"></param>
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
