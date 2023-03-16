using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.EsiEve;
using System.Linq;

namespace Arkenstone.API.Services
{
    public class AllianceService
    {
        private ArkenstoneContext _context;

        public AllianceService(ArkenstoneContext context)
        {
            _context = context;
        }

        public Alliance GetOrCreate(int id)
        {
            Alliance alliance = _context.Alliances.FirstOrDefault(x => x.Id == id);
            if (alliance == null)
            {
                alliance = new Alliance();
                alliance.Id = id;
                alliance.Name = EsiAlliance.GetName(alliance.Id);
                _context.Alliances.Add(alliance);
                _context.SaveChanges();
            }
            return alliance;
        }
 

    }
}
