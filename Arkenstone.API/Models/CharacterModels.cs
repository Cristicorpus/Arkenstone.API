using Arkenstone.Entities.DbSet;
using System.Collections.Generic;

namespace EveMiningFleet.API.Models
{
    public class CharacterModel
    {
        public CharacterModel(Character target)
        {
            this.Id = target.Id;
            this.Name = target.Name;
            this.Coorporation = target.Corporation;
            this.Alliance = target.Alliance;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Corporation Coorporation { get; set; }
        public Alliance Alliance { get; set; }

    }
    public class MainCharacterModel
    {
        public MainCharacterModel(Character target)
        {
            this.main = new CharacterModel(target);
            this.AltCharacter = new List<CharacterModel>();
        }

        public CharacterModel main { get; set; }
        public List<CharacterModel> AltCharacter { get; set; }

    }
}
