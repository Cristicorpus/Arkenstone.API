using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.EsiEve;
using System.Linq;

namespace Arkenstone.API.Services
{
    public class CorporationService
    {
        private ArkenstoneContext _context;

        public CorporationService(ArkenstoneContext context)
        {
            _context = context;
        }
        public Corporation GetOrCreate(int id)
        {
            Corporation corporation = _context.Corporations.FirstOrDefault(x => x.Id == id);
            if (corporation == null)
            {
                corporation = new Corporation();
                corporation.Id = id;
                corporation.Name = EsiCorporation.GetName(corporation.Id);
                _context.Corporations.Add(corporation);
                _context.SaveChanges();
            }
            return corporation;
        }

    }
}
