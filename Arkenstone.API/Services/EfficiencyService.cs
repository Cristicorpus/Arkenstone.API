using Arkenstone.API.Models;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.Efficiency;

namespace Arkenstone.API.Services
{
    public class EfficiencyService
    {
        private ArkenstoneContext _context;

        public EfficiencyService(ArkenstoneContext context)
        {
            _context = context;
        }

        public EfficiencyModel GetEfficiencyModelFromLocation(Location location, Item Item)
        {

            var returnModel = new EfficiencyModel();

            decimal StructureEfficiency = EfficiencyStructure.GetMEEfficiencyFromStation(_context, location);
            EfficiencyStructureRigsEffect RigsEfficiency = EfficiencyStructure.GetMEEfficiencyFromRigs(_context, location, Item);

            returnModel.MEefficiency = (StructureEfficiency * RigsEfficiency.MeEfficiency);
            returnModel.Station = new LocationModel(location);
            returnModel.rigsEffect = RigsEfficiency.rigsManufacturings;
            
            return returnModel;
        }


        public decimal GetEfficiencyFromLocation(long locationId, int ItemId)
        {
            LocationService locationService = new LocationService(_context);
            ItemService itemService = new ItemService(_context);

            var location = locationService.Get(locationId);
            var item = itemService.Get(ItemId);

            decimal StructureEfficiency = EfficiencyStructure.GetMEEfficiencyFromStation(_context, location);
            var RigsEfficiency = EfficiencyStructure.GetMEEfficiencyFromRigs(_context, location, item);

            return (StructureEfficiency * RigsEfficiency.MeEfficiency);
        }



    }
}
