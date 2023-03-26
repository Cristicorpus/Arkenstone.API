using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.BusinessException;
using ESI.NET.Models.SSO;
using Microsoft.EntityFrameworkCore;

namespace Arkenstone.Logic.Repository
{
    public class CharacterRepository
    {
        private ArkenstoneContext _context;

        public CharacterRepository(ArkenstoneContext context)
        {
            _context = context;
        }
        private IQueryable<Character> GetCore()
        {
            return _context.Characters.Include("Corporation").Include("Alliance").Include("CharacterMain");
        }

        public Character GetAndUpdate(int Id, int corpId, 
            int allianceId, string name, 
            string token, string refreshToken)
        {
            Character character = GetCore().FirstOrDefault(x => x.Id == Id);
            if (character == null)
            {
                character = new Character();
                character.Id = Id;
                character.CharacterMainId = Id;
                _context.Characters.Add(character);
            }
            character.AllianceId = allianceId;
            character.CorporationId = corpId;
            //on met a jours les informations du player.
            character.Name = name;
            character.Token = token;
            character.RefreshToken = refreshToken;
            _context.SaveChanges();
            return character;
        }
        public Character Get(int id)
        {
            Character character = GetCore().FirstOrDefault(x => x.Id == id);
            if (character == null)
                throw new NotFound("Character");
            else
                return character;
        }
        public Character GetByName(string name)
        {
            Character character = GetCore().FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
            if (character == null)
                throw new NotFound("Character");
            else
                return character;
        }
        public List<Character> GetListFromMain(int mainId)
        {
            return GetCore().Where(x => x.Id != mainId && x.CharacterMainId == mainId).ToList();
        }

        public Character CharacterSetMain(Character child, int mainID)
        {
            child.CharacterMainId = mainID;
            _context.SaveChanges();
            return child;
        }
        public Character CharacterSetMain(int id, int mainID)
        {
            Character character = _context.Characters.FirstOrDefault(x => x.Id == id);
            if (character != null)
                character.CharacterMainId = mainID;
            _context.SaveChanges();

            return character;
        }

        public void BulkUpdateMainId(int oldMainId, int mainId)
        {
            foreach (var character in GetCore().Where(x => x.CharacterMainId == oldMainId))
                character.CharacterMainId = mainId;
            _context.SaveChanges();
        }

        public void ClearToken(Character character)
        {
            character.Token = "";
            character.RefreshToken = "";
            character.CharacterMainId = character.Id;
            ResetMain(character);
        }
        public void ResetMain(Character character)
        {
            character.CharacterMainId = character.Id;
        }


    }
}
