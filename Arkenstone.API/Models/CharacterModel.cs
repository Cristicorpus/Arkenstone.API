using Arkenstone.Entities.DbSet;


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
}
