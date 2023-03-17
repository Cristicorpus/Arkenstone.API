using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Arkenstone.API.Services
{
    public class ProdAchatService
    {
        private ArkenstoneContext _context;

        public ProdAchatService(ArkenstoneContext context)
        {
            _context = context;
        }
        private IQueryable<ProdAchat> GetCore()
        {
            return _context.ProdAchats.Include("Item").Include("Location").Include("ProdAchatParent").Include("ProdAchatEnfants");
        }

        public ProdAchat Get(long id)
        {
            return GetCore().FirstOrDefault(x => x.Id == id);
        }

    }
}
