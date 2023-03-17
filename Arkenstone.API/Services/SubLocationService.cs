using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Arkenstone.API.Services
{
    public class SubLocationService
    {
        private ArkenstoneContext _context;

        public SubLocationService(ArkenstoneContext context)
        {
            _context = context;
        }

        private IQueryable<SubLocation> RequestSubLoc()
        {
            return _context.SubLocations.AsQueryable();
        }
        public SubLocation GetFirstOrDefault(int id)
        {
            return RequestSubLoc().FirstOrDefault(x => x.Flag != "Office" && x.Id == id);
        }
        public List<SubLocation> ListSubLocationCorp(int corpID)
        {
            return RequestSubLoc().Where(x => x.Flag!="Office" && x.CorporationId == corpID).ToList();
        }
        public List<SubLocation> GetSubLocationByCorp(int CorpId)
        {
            return RequestSubLoc().Where(x => x.Flag != "Office" && x.CorporationId == CorpId).ToList();
        }
        public List<SubLocation> GetSubLocationByLocation(int CorpId, long location)
        {
            return RequestSubLoc().Where(x => x.Flag != "Office" && x.CorporationId == CorpId && x.LocationId == location).ToList();
        }
        public List<SubLocation> GetSubLocationBySubLocationId(int Sublocation)
        {
            return RequestSubLoc().Where(x => x.Flag != "Office" && x.Id == Sublocation).ToList();
        }

        public void EditSubLocation(int Sublocation,bool toAnalyse)
        {
            _context.SubLocations.First(x=>x.Id ==Sublocation).IsAssetAnalysed = toAnalyse;
            _context.SaveChanges();
        }




    }
}
