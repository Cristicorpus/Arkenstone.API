using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.EsiEve;

namespace Arkenstone.Logic.Repository
{
    public class CorporationRepository
    {
        private ArkenstoneContext _context;

        public CorporationRepository(ArkenstoneContext context)
        {
            _context = context;
        }
        public Corporation GetAndCreate(int id)
        {
            Corporation? corporation = _context.Corporations.FirstOrDefault(x => x.Id == id);
            if (corporation == null)
                corporation = Insert(id);
            return corporation;
        }

        public Corporation Insert(int id)
        {
            var corporation = new Corporation();
            corporation.Id = id;
            corporation.Name = EsiCorporation.GetName(corporation.Id);
            _context.Corporations.Add(corporation);
            _context.SaveChanges();
            return corporation;
        }

    }
}
