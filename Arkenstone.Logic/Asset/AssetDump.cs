using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.Esi;
using ESI.NET.Models.Location;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Location = Arkenstone.Entities.DbSet.Location;

namespace Arkenstone.Logic.Asset
{
    public static class AssetDump
    {
        public static int ReloadSecondeSpan = 3600;
        public static void ReloadAllItemsAsynctask()
        {
            _ = ReloadAllItemsAsync();
        }
        public static async Task ReloadAllItemsAsync()
        {
            var eveEsi = new EveEsi_Connexion();


            try
            {
                var _dbConnectionString = System.Environment.GetEnvironmentVariable("DB_DATA_connectionstring");
                var options = new DbContextOptionsBuilder<ArkenstoneContext>().UseMySql(_dbConnectionString, ServerVersion.AutoDetect(_dbConnectionString)).Options;
                using (ArkenstoneContext context = new ArkenstoneContext(options))
                {
                    int coorporationId = 0;
                    //recuperation de tout les asset en HARD ESI
                    var AllCoorpoAsset = new List<ESI.NET.Models.Assets.Item>();
                    foreach (var item in context.Characters)
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
                    
                    coorporationId = eveEsi.authorizedCharacterData.CorporationID;

                    //recuperation des bureau dans les stations
                    var AllCoorpoAssetOffice = AllCoorpoAsset.Where(x => x.TypeId==27).ToList();
                    //recuperation des asset dans les offices
                    var AllCoorpoAssetHangar = AllCoorpoAsset.Where(x => x.LocationFlag.Contains("CorpSAG")).ToList();
                    //recuperation des asset dans les container dans les hangars
                    var AllCoorpoAssetContainer = AllCoorpoAsset.Where(x => x.LocationFlag == "AutoFit" && x.LocationId != 2004).ToList();

                    //ajout des stations et structure inconnu
                    foreach (var item in AllCoorpoAssetOffice.Select(x => new { x.LocationId, x.ItemId }).Distinct())
                    {
                        if (context.Locations.Find(item.LocationId) == null)
                        {
                            //TODO ajouter le scope pour lire les structure
                            //var temp = await eveEsi.EsiClient.Universe.Structure(item);
                            //var newStation = new Location() { Id = item, Name = temp.Data.Name };

                            var newStation = new Location() { Id = item.LocationId, Name = "" };
                            context.Locations.Add(newStation);
                            context.SaveChanges();
                        }
                            
                    }
                    
                    //ajout des hangars de coorp dans les stations/structure inconnu
                    foreach (var item in AllCoorpoAssetHangar.Select(x=> new { x.LocationFlag,x.LocationId }).Distinct())
                    {
                        var office = AllCoorpoAssetOffice.FirstOrDefault(x => x.ItemId == item.LocationId);
                        if (office != null)
                        {
                            var hangar = context.SubLocations.FirstOrDefault(x => x.LocationId == office.LocationId && x.Flag == item.LocationFlag);
                            if ( hangar == null)
                            {
                                context.SubLocations.Add(new SubLocation() { LocationId = office.LocationId, Flag = item.LocationFlag, CorporationId = coorporationId, IsAssetAnalysed = false });
                                context.SaveChanges();
                            }
                        }
                    }

                    // analyse reel des asset pour integration a la DB
                    List<Inventory> Assets = new List<Inventory>();
                    foreach (var itemHangar in AllCoorpoAssetHangar)
                    {
                        var office = AllCoorpoAssetOffice.FirstOrDefault(x => x.ItemId == itemHangar.LocationId);

                        if (office!=null)
                        {
                            var subLocationDb = context.SubLocations.FirstOrDefault(x=> x.LocationId == office.LocationId && x.Flag==itemHangar.LocationFlag);

                            if (subLocationDb != null && subLocationDb.IsAssetAnalysed)
                            {
                                var assetHanger = Assets.FirstOrDefault(x => x.ItemId == itemHangar.TypeId && x.SubLocationId == subLocationDb.Id);
                                if (assetHanger == null)
                                {
                                    assetHanger = new Inventory() { ItemId = itemHangar.TypeId, SubLocationId = subLocationDb.Id};
                                    Assets.Add(assetHanger);
                                }
                                assetHanger.Quantity += itemHangar.Quantity;

                                foreach (var itemContainer in AllCoorpoAssetContainer.Where(x => x.LocationId == itemHangar.ItemId))
                                {
                                    var assetContainer = Assets.FirstOrDefault(x => x.ItemId == itemContainer.TypeId && x.SubLocationId == subLocationDb.Id);
                                    if (assetContainer == null)
                                    {
                                        assetContainer = new Inventory() { ItemId = itemContainer.TypeId, SubLocationId = subLocationDb.Id};
                                        Assets.Add(assetContainer);
                                    }
                                    assetContainer.Quantity += itemContainer.Quantity;
                                }
                            }
                        }
                        
                    }

                    var AssetsWithItemKnow = Assets.Where(x => context.Items.Any(y => y.Id == x.ItemId));


                    context.Database.ExecuteSqlRaw("DELETE Inventorys FROM Inventorys INNER JOIN Sublocations ON Inventorys.SubLocationId = Sublocations.Id WHERE Sublocations.CorporationID = "+ coorporationId.ToString());
                    //on ajoute que les items qu on connais en attendant l update des items par fuzzwork
                    context.Inventorys.AddRange(AssetsWithItemKnow);
                    context.SaveChanges();

                    Debug.WriteLine("YHOLO");





                }
            }
            catch (Exception ex)
            {

            }

        }
    }
}
