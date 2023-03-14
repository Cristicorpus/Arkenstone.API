using Arkenstone.Database;
//using Arkenstone.Database.Seat;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Arkenstone.Functions
{
    public class SeatRefresh
    {

        public static void RereshInventory()
        {
            ClassLog.writeLog("SeatRefresh/RereshInventory => Begin");

            using (var contextSeat = SeatContext.GetInstance())
            using (var contextArkenstone = ArkenstoneContext.GetInstance())
            {
                //On Supprime L'ancien
                contextArkenstone.Database.ExecuteSqlRaw("DELETE FROM InventoryBlueprints");
                contextArkenstone.Database.ExecuteSqlRaw("DELETE FROM Inventorys");
                contextArkenstone.Database.ExecuteSqlRaw("DELETE FROM Locations");

                List<int> idItems = contextArkenstone.Items.Select(r => r.Id).ToList();
                
                List<ulong> idLocation = contextSeat.Blueprints
                    .Where(r => idItems.Contains(r.type_id) && r.corporation_id == (ulong)CorporationId.Cristicorpus)
                    .Select(r => r.location_id)
                    .Distinct()
                    .ToList();

                idLocation = idLocation.Concat(contextSeat.Assets
                .Where(r => idItems.Contains(r.type_id) && r.corporation_id == (ulong)CorporationId.Cristicorpus && !idLocation.Contains(r.location_id))
                .Select(r => r.location_id)
                .Distinct()
                .ToList()).ToList();

                List<CorporationBlueprint> BlueprintSeat = contextSeat.Blueprints.Where(r => idItems.Contains(r.type_id)).Select(r => r).ToList();
                List<CorporationAsset> AssetSeat = contextSeat.Assets.Where(r => idItems.Contains(r.type_id)).Select(r => r).ToList();
                    
                //List<ulong> idLocation2 = BlueprintSeat.FindAll(r => idItems.Contains(r.type_id) && r.corporation_id == (ulong)CorporationId.Cristicorpus).Select(r => r.location_id).ToList();

                foreach (var id in idLocation)
                {
                    contextArkenstone.Locations.Add(new Location()
                    {
                        Id = id,

                        //A remplir
                        CanProd = false,
                        CanReact = false,
                        CanReprocess = false,
                    });
                }
                contextArkenstone.SaveChanges();

                foreach(var blueprintSeat in BlueprintSeat)
                {
                    if (idItems.Contains(blueprintSeat.type_id) && blueprintSeat.corporation_id == (ulong)CorporationId.Cristicorpus)
                        contextArkenstone.InventoryBlueprints.Add(new InventoryBlueprint()
                        {
                            Id = blueprintSeat.item_id,
                            ItemId = blueprintSeat.type_id,
                            CycleNumber = blueprintSeat.runs,
                            MaterialEfficiency = blueprintSeat.material_efficiency,
                            TimeEfficiency = blueprintSeat.material_efficiency,
                            LocationId = blueprintSeat.location_id,
                        });
                }
                contextArkenstone.SaveChanges();

                foreach(var assetSeat in AssetSeat)
                {
                    if (idItems.Contains(assetSeat.type_id) && assetSeat.corporation_id == (ulong)CorporationId.Cristicorpus)
                        contextArkenstone.Inventorys.Add(new Inventory()
                        {
                            Id = assetSeat.item_id,
                            ItemId = assetSeat.type_id,
                            LocationId = assetSeat.location_id,
                            Quantity = assetSeat.quantity,
                        });
                }
                contextArkenstone.SaveChanges();

                ClassLog.writeLog("SeatRefresh/RereshInventory => End");
            }
        }
        
        enum CorporationId
        {
            Cristicorpus = 98571968,
        }
    }
}
