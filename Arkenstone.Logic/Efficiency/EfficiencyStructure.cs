using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkenstone.Logic.Efficiency
{
    public static class EfficiencyStructure
    {

        public static decimal GetMEEfficiencyFromStation(ArkenstoneContext _context, Location Location)
        {
            decimal MeEfficiency = 1;

            if (Location.StructureTypeId.HasValue)
            {
                var structureType = _context.StructureTypes.Find(Location.StructureTypeId);
                if (structureType != null)
                {
                    if(structureType.MaterialEffect==0)
                        MeEfficiency = 1;
                    else
                        MeEfficiency = structureType.MaterialEffect;
                }
            }

            return MeEfficiency;
        }
        public static EfficiencyStructureRigsEffect GetMEEfficiencyFromRigs(ArkenstoneContext _context, Location Location, Item Item)
        {
            EfficiencyStructureRigsEffect returnValue = new EfficiencyStructureRigsEffect();

            if (Location.StructureTypeId.HasValue)
            {
                var AllMarketGroupFromItem = GetAllGroupMarketIdFromItemId(_context, Item.MarketGroupId);

                var request = _context.LocationRigsManufacturings.Include("RigsManufacturing");
                foreach (var rigItem in request.Where(x => x.LocationId == Location.Id))
                {
                    if (!IsImpactedByRigs(AllMarketGroupFromItem, rigItem.RigsManufacturing))
                        continue;
                    decimal StatusMultiplier = 1;
                    if (Location.Security >= 5)
                        StatusMultiplier = rigItem.RigsManufacturing.MultiplierHS;
                    if (Location.Security < 5 && Location.Security > 0)
                        StatusMultiplier = rigItem.RigsManufacturing.MultiplierLS;
                    if (Location.Security <= 0)
                        StatusMultiplier = rigItem.RigsManufacturing.MultiplierNS;

                    returnValue.MeEfficiency = 1 + ((rigItem.RigsManufacturing.MaterialEffect / 100) * StatusMultiplier);
                    returnValue.rigsManufacturings.Add(rigItem.RigsManufacturing);
                }
            }

            return returnValue;
        }
        private static List<int> GetAllGroupMarketIdFromItemId(ArkenstoneContext _context, int ItemId)
        {
            var returnList = new List<int>();
            int? parentID = ItemId;
            do
            {
                var marketGroup = _context.MarketGroupTrees.Find(parentID);
                if (marketGroup != null)
                {
                    returnList.Add(marketGroup.Id);
                    parentID = marketGroup.ParentId;
                }
                else
                    parentID = null;

            } while (parentID.HasValue);

            return returnList;
        }
        private static bool IsImpactedByRigs(List<int> MarketGroup, RigsManufacturing Rig)
        {
            var rigMarketGroupEffect = Rig.MarketIdEffect.Split('*');
            var rigMarketGroupIgnore = Rig.MarketIdNotEffect.Split('*');

            foreach (var rigMarketGroup in rigMarketGroupIgnore)
            {
                if (MarketGroup.Contains(int.Parse(rigMarketGroup)))
                    return false;
            }
            foreach (var rigMarketGroup in rigMarketGroupEffect)
            {
                if (MarketGroup.Contains(int.Parse(rigMarketGroup)))
                    return true;
            }
            return false;
        }


    }

    public class EfficiencyStructureRigsEffect
    {
        public decimal MeEfficiency { get; set; } = 1;
        public List<RigsManufacturing> rigsManufacturings { get; set; } = new List<RigsManufacturing>();
    }

}
