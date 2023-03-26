using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.Esi;
using Microsoft.EntityFrameworkCore;

namespace Arkenstone.Logic.Asset
{
    public static class AssetDump
    {
        public static void ReloadAllItemsAsynctask()
        {
            _ = ReloadAllItemsAsync();
        }
        public static async Task ReloadAllItemsAsync()
        {

            var datebegin = DateTime.Now;
            Logs.ClassLog.writeLog("ReloadAllItemsAsync => Begin...");

            using (ArkenstoneContext context = ArkenstoneContext.GetContextWithDefaultOption())
            {
                //recuperation de tout les asset en HARD ESI
                foreach (var item in context.Corporations)
                {
                    try
                    {
                        await ReloadItemsFromSpecificCorpAsync(context, item.Id);
                    }
                    catch (Exception ex)
                    {
                        Logs.ClassLog.writeLog("ReloadAllItemsAsync => for " + item.Id + " : " + item.Name + " , error ");
                    }
                }
            }
            Logs.ClassLog.writeLog("ReloadAllItemsAsync => " + DateTime.Now.Subtract(datebegin).ToString() + " Secondes");

        }

        public static async Task ReloadItemsFromSpecificCorpAsync(ArkenstoneContext context , int corpId)
        {
            Logs.ClassLog.writeLog("ReloadItemsFromSpecificCorpAsync( "+ corpId + " ) => Begin...");
            try
            {
                var eveEsi = new EveEsiConnexion();
                //recuperation de tout les asset en HARD ESI
                var AllCoorpoAsset = new List<ESI.NET.Models.Assets.Item>();
                foreach (var item in context.Characters.Where(x => x.CorporationId == corpId))
                {
                    try
                    {
                        await eveEsi.RefreshConnection(item.RefreshToken);
                        int cursor = 0;
                        do
                        {
                            cursor++;
                            var OrdersReponse = await eveEsi.EsiClient.Assets.ForCorporation(cursor);
                            if (OrdersReponse.Data != null)
                                AllCoorpoAsset.AddRange(OrdersReponse.Data);
                            else
                                break;
                        } while (cursor < 100);

                        if (AllCoorpoAsset.Count > 0)
                            break;
                    }
                    catch (Exception)
                    {

                    }
                }
                
                
                if (AllCoorpoAsset.Count <= 0)
                    return;

                Logs.ClassLog.writeLog("ReloadItemsFromSpecificCorpAsync( " + corpId + " ) => Use " + eveEsi.authorizedCharacterData.CharacterName + " for refresh");
                Logs.ClassLog.writeLog("ReloadItemsFromSpecificCorpAsync( " + corpId + " ) => AllCoorpoAsset.count()= " + AllCoorpoAsset.Count().ToString());

                //recuperation des bureau dans les stations
                List<ESI.NET.Models.Assets.Item> AllCoorpoAssetOffice = AllCoorpoAsset.Where(x => x.TypeId == 27).ToList();
                //recuperation des asset dans les offices
                var AllCoorpoAssetHangar = AllCoorpoAsset.Where(x => x.LocationFlag.Contains("CorpSAG")).ToList();

                //ajout des stations et structure inconnu
                foreach (var item in AllCoorpoAssetOffice.Select(x => new { x.LocationId, x.ItemId }).Distinct())
                {
                    if (context.Locations.Find(item.LocationId) == null)
                    {

                        var newStation = new Location() { Id = item.LocationId };

                        if (item.LocationId >= 1000000000)
                        {
                            var tempStructure = await eveEsi.EsiClient.Universe.Structure(item.LocationId);
                            newStation.Name = tempStructure.Data.Name;
                            newStation.StructureTypeId = tempStructure.Data.TypeId;
                            var tempSecurity = await eveEsi.EsiClient.Universe.System(tempStructure.Data.SolarSystemId);
                            newStation.Security = tempSecurity.Data.SecurityStatus;
                            newStation.SolarSystemId = tempStructure.Data.SolarSystemId;
                        }
                        else
                        {
                            var temp = await eveEsi.EsiClient.Universe.Station((int)item.LocationId);
                            newStation.Name = temp.Data.Name;
                            newStation.StructureTypeId = 2071;
                            newStation.SolarSystemId = temp.Data.SystemId;
                        }

                        context.Locations.Add(newStation);
                        context.SaveChanges();
                    }

                    if (context.SubLocations.FirstOrDefault(x => x.LocationId == item.LocationId && x.Flag == "Office") == null)
                    {
                        var newSubStation = new SubLocation() { LocationId = item.LocationId, Flag = "Office", IsAssetAnalysed = false, CorporationId = corpId };
                        context.SubLocations.Add(newSubStation);
                        context.SaveChanges();
                    }
                }

                //ajout des hangars de coorp dans les stations/structure inconnu
                foreach (var item in AllCoorpoAssetHangar.Select(x => new { x.LocationFlag, x.LocationId }).Distinct())
                {
                    var office = AllCoorpoAssetOffice.FirstOrDefault(x => x.ItemId == item.LocationId);
                    if (office != null)
                    {
                        var hangar = context.SubLocations.FirstOrDefault(x => x.LocationId == office.LocationId && x.Flag == item.LocationFlag);
                        if (hangar == null)
                        {
                            context.SubLocations.Add(new SubLocation() { LocationId = office.LocationId, Flag = item.LocationFlag, CorporationId = corpId, IsAssetAnalysed = false });
                            context.SaveChanges();
                        }
                    }
                }

                // analyse reel des asset pour integration a la DB
                List<Inventory> AssetsValid = new List<Inventory>();
                foreach (var itemHangar in AllCoorpoAssetHangar)
                {
                    var office = AllCoorpoAssetOffice.FirstOrDefault(x => x.ItemId == itemHangar.LocationId);
                    if (office != null)
                    {
                        var subLocationDb = context.SubLocations.FirstOrDefault(x => x.LocationId == office.LocationId && x.Flag == itemHangar.LocationFlag);

                        if (subLocationDb != null && subLocationDb.IsAssetAnalysed)
                        {
                            var assetHanger = AssetsValid.FirstOrDefault(x => x.ItemId == itemHangar.TypeId && x.SubLocationId == subLocationDb.Id);
                            if (assetHanger == null)
                            {
                                assetHanger = new Inventory() { ItemId = itemHangar.TypeId, SubLocationId = subLocationDb.Id };
                                AssetsValid.Add(assetHanger);
                            }
                            assetHanger.Quantity += itemHangar.Quantity;

                            foreach (var itemContainer in AllCoorpoAsset.Where(x => x.LocationId == itemHangar.ItemId))
                            {
                                var assetContainer = AssetsValid.FirstOrDefault(x => x.ItemId == itemContainer.TypeId && x.SubLocationId == subLocationDb.Id);
                                if (assetContainer == null)
                                {
                                    assetContainer = new Inventory() { ItemId = itemContainer.TypeId, SubLocationId = subLocationDb.Id };
                                    AssetsValid.Add(assetContainer);
                                }
                                assetContainer.Quantity += itemContainer.Quantity;
                            }
                        }
                    }

                }


                Logs.ClassLog.writeLog("ReloadItemsFromSpecificCorpAsync( " + corpId + " ) => AssetsValid.count()= " + AssetsValid.Count().ToString());
                var AssetsWithItemKnow = AssetsValid.Where(x => context.Items.Any(y => y.Id == x.ItemId)).ToList();
                Logs.ClassLog.writeLog("ReloadItemsFromSpecificCorpAsync( " + corpId + " ) => AssetsWithItemKnow.count()= " + AssetsValid.Count().ToString());

                context.Database.ExecuteSqlRaw("DELETE Inventorys FROM Inventorys INNER JOIN SubLocations ON Inventorys.SubLocationId = SubLocations.Id WHERE SubLocations.CorporationID = " + corpId.ToString());
                context.SubLocations.Where(x => x.CorporationId == corpId).ToList().ForEach(x => x.LastUpdated = null);
                context.SubLocations.Where(x => x.CorporationId == corpId && x.IsAssetAnalysed).ToList().ForEach(x => x.LastUpdated = DateTime.Now);
                context.Inventorys.AddRange(AssetsWithItemKnow);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Logs.ClassLog.writeLog("ReloadItemsFromSpecificCorpAsync( " + corpId + " ) => Error= ");
                Logs.ClassLog.writeException(ex);
            }

        }


    }
}
