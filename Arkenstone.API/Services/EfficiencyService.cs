using Arkenstone.API.Models;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;

namespace Arkenstone.API.Services
{
    public class EfficiencyService : BaseService
    {
        public EfficiencyService(ArkenstoneContext context) : base(context)
        {

        }

        public EfficiencyModel GetEfficiencyModelFromLocation(Location location, Item Item)
        {

            var returnModel = new EfficiencyModel();

            var StructureEfficiency = efficiencyRepository.GetEfficiency(location, Item);
            
            returnModel.MEefficiency = StructureEfficiency.GlobalMaterialEfficiency;
            returnModel.Station = new LocationModel(StructureEfficiency.Location);
            returnModel.rigsEffect = StructureEfficiency.RigsEfficiency.RigsEffect;
            
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


        public decimal GetBestEfficiencyFromLocation(int corpId, int ItemId)
        {
            LocationService locationService = new LocationService(_context);
            ItemService itemService = new ItemService(_context);
            var item = itemService.Get(ItemId);

            decimal bestEfficiency = 1;

            foreach (var location in locationService.GetList(corpId))
            {
                decimal StructureEfficiency = EfficiencyStructure.GetMEEfficiencyFromStation(_context, location);
                var RigsEfficiency = EfficiencyStructure.GetMEEfficiencyFromRigs(_context, location, item);

                if (bestEfficiency > (StructureEfficiency * RigsEfficiency.MeEfficiency))
                    bestEfficiency = (StructureEfficiency * RigsEfficiency.MeEfficiency);
            }

            return bestEfficiency;
        }



    }
}
