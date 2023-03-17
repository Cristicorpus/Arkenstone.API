using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.BusinessException;
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

        public List<SubLocation> GetList(int corpID)
        {
            var temp = RequestSubLoc().Where(x => x.Flag != "Office" && x.CorporationId == corpID).ToList();
            if (temp.Count() == 0)
                throw new NoContent("SubLocation");
            return temp;
        }
        public List<SubLocation> GetListFromLocation(int corpID,long locationId)
        {
            var temp = RequestSubLoc().Where(x => x.Flag != "Office"&& x.CorporationId == corpID && x.LocationId == locationId).ToList();
            if (temp.Count() == 0)
                throw new NoContent("SubLocation");
            return temp;
        }
        public SubLocation Get(long subLocationID)
        {
            SubLocation temp = RequestSubLoc().FirstOrDefault(x => x.Flag != "Office" && x.Id == subLocationID);
            if (temp == null)
                throw new NotFound("SubLocation");
            return temp;
        }

        public void EditSubLocation(long Sublocation,bool toAnalyse)
        {
            _context.SubLocations.First(x=>x.Id ==Sublocation).IsAssetAnalysed = toAnalyse;
            _context.SaveChanges();
        }




    }
    public static class SubLocationExtension
    {
        public static SubLocation ThrowNotAuthorized(this SubLocation subLocation, int corpId)
        {
            if (subLocation.CorporationId != corpId)
                throw new NotAuthorized();
            return subLocation;
        }
    }
}
