using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.EsiEve;

namespace Arkenstone.Logic.Repository
{
    public class AllianceRepository
    {
        private ArkenstoneContext _context;

        public AllianceRepository(ArkenstoneContext context)
        {
            _context = context;
        }

        public Alliance GetAndCreate(int id)
        {
            Alliance? alliance = _context.Alliances.FirstOrDefault(x => x.Id == id);
            if (alliance == null)
                alliance = Insert(id);
            return alliance;
        }

        public Alliance Insert(int id)
        {
            var alliance = new Alliance();
            alliance.Id = id;
            alliance.Name = EsiAlliance.GetName(alliance.Id);
            _context.Alliances.Add(alliance);
            _context.SaveChanges();
            return alliance;
        }


    }
}
