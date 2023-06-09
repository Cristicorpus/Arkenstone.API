﻿using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.GlobalTools;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography;

namespace Arkenstone.Logic.BulkUpdate
{
    public static class FuzzWorkDump
    {
        //public delegate List<U> delegateCSV<T, U>(List<T> list);
        private static string _folderPathESI = "Applicatif/fuzzwork/";
        public static void CheckDumpAsynctask()
        {
            _ = CheckDump();
        }
        public static async Task CheckDump()
        {
            var datebegin = DateTime.Now;

            if (!Directory.Exists(_folderPathESI))
                Directory.CreateDirectory(_folderPathESI);

            if (NewEsiDump())
            {
                Logs.ClassLog.writeLog("NewEsiDump => " + DateTime.Now.Subtract(datebegin).ToString() + " Secondes");
                
                datebegin = DateTime.Now;
                InsertEsiSDE_Activity();
                Logs.ClassLog.writeLog("InsertEsiSDE_Activity => " + DateTime.Now.Subtract(datebegin).ToString() + " Secondes");

                datebegin = DateTime.Now;
                await RigsDump.UpdateRigs_Activity();
                Logs.ClassLog.writeLog("UpdateRigs_Activity => " + DateTime.Now.Subtract(datebegin).ToString() + " Secondes");
            }

        }

        private static bool NewEsiDump()
        {
            bool response = false;
            Logs.ClassLog.writeLog("NewEsiDump => Check if dump ESI is different");
            Dictionary<string, Uri> allfildownload = new Dictionary<string, Uri>();

            allfildownload.Add(_folderPathESI + "industryActivityProducts.csv", new Uri("https://www.fuzzwork.co.uk/dump/latest/industryActivityProducts.csv"));
            allfildownload.Add(_folderPathESI + "industryActivityMaterials.csv", new Uri("https://www.fuzzwork.co.uk/dump/latest/industryActivityMaterials.csv"));
            allfildownload.Add(_folderPathESI + "industryActivity.csv", new Uri("https://www.fuzzwork.co.uk/dump/latest/industryActivity.csv"));
            allfildownload.Add(_folderPathESI + "invMarketGroups.csv", new Uri("https://www.fuzzwork.co.uk/dump/latest/invMarketGroups.csv"));
            allfildownload.Add(_folderPathESI + "invTypes.csv", new Uri("https://www.fuzzwork.co.uk/dump/latest/invTypes.csv"));

            foreach (var item in allfildownload)
            {
                byte[] oldchecksum = { };
                byte[] newchecksum = { };


                //on calcul le checksum precedent si il exist
                if (File.Exists(item.Key))
                {
                    using var md5 = MD5.Create();
                    using (var stream = File.OpenRead(item.Key))
                    {
                        oldchecksum = md5.ComputeHash(stream);
                    }
                }

                //on recupere le checksum du nouveau
                using (WebClient webclient = new WebClient())
                {
                    webclient.Headers.Add("User-Agent: Other");
                    using var md5 = MD5.Create();
                    using (var stream = webclient.OpenRead(item.Value))
                    {
                        newchecksum = md5.ComputeHash(stream);
                    }
                }

                if (!newchecksum.SequenceEqual(oldchecksum))
                {
                    if (File.Exists(item.Key))
                        File.Delete(item.Key);

                    using (WebClient webclient = new WebClient())
                    {
                        webclient.Headers.Add("User-Agent: Other");
                        webclient.DownloadFile(item.Value, item.Key);
                    }
                    response = true;
                }
            }

            if (response)
                Logs.ClassLog.writeLog("NewEsiDump => True");
            else
                Logs.ClassLog.writeLog("NewEsiDump => False");

            return response;
        }

        private static void InsertEsiSDE_Activity()
        {
            try
            {

                Logs.ClassLog.writeLog("InsertEsiSDE => Reinsertion des information ESI dans les recettes");

                using (ArkenstoneContext context = ArkenstoneContext.GetContextWithDefaultOption())
                {
                    List<ActivityProductsCsv> ActivityProductsCsvrecords;
                    List<ActivityMaterialsCsv> ActivityMaterialsCsvrecords;
                    List<IndustryActivityCsv> industryActivityCsvrecords;
                    List<MarketGroupActivityCsv> MarketGroupsCsvrecords;
                    List<InvTypesCsv> invTypesCsvrecords;

                    ActivityProductsCsvrecords = CsvTools.ReadCsv<ActivityProductsCsv>(_folderPathESI + "industryActivityProducts.csv");
                    ActivityMaterialsCsvrecords = CsvTools.ReadCsv<ActivityMaterialsCsv>(_folderPathESI + "industryActivityMaterials.csv");
                    industryActivityCsvrecords = CsvTools.ReadCsv<IndustryActivityCsv>(_folderPathESI + "industryActivity.csv");
                    MarketGroupsCsvrecords = CsvTools.ReadCsv<MarketGroupActivityCsv>(_folderPathESI + "invMarketGroups.csv", new MarketGroupActivityCsvMap());
                    invTypesCsvrecords = CsvTools.ReadCsv<InvTypesCsv>(_folderPathESI + "invTypes.csv", new InvTypesCsvMap());



                    //on supprimes les recettes et les matériaux présent

                    //MAJ item
                    foreach (var itemCsv in invTypesCsvrecords)
                    {
                        var itemDb = context.Items.FirstOrDefault(x => x.Id == itemCsv.typeID);
                        if (itemDb == null)
                        {
                            itemDb = new Item() { Id = itemCsv.typeID, PriceBuy = 0, PriceSell = 0, PriceAdjustedPrice = 0 };
                            context.Items.Add(itemDb);
                        }

                        itemDb.Name = itemCsv.typeName;
                        itemDb.Published = itemCsv.published;
                        itemDb.MarketGroupId = itemCsv.marketGroupID;
                    }
                    context.SaveChanges();

                    //MAJ MarketGroup
                    context.Database.ExecuteSqlRaw("DELETE FROM MarketGroupTrees");
                    foreach (var item in MarketGroupsCsvrecords)
                    {
                        context.MarketGroupTrees.Add(
                           new MarketGroupTree()
                           {
                               Id = item.marketGroupID,
                               ParentId = item.parentGroupID,
                               Name = item.marketGroupName
                           });
                    }
                    context.SaveChanges();

                    //MAJ recettes
                    context.Database.ExecuteSqlRaw("DELETE FROM Recipes");
                    foreach (var item in ActivityProductsCsvrecords.Where(x => (TypeRecipeEnum)x.activityID == TypeRecipeEnum.Manufacturing && context.Items.Any(y => y.Published == true && y.Id == x.productTypeID)))
                    {
                        var Activity = industryActivityCsvrecords.Find(x => (TypeRecipeEnum)x.activityID == TypeRecipeEnum.Manufacturing && x.typeID == item.typeID);

                        var recipeDb = new Recipe()
                        {
                            Id = item.typeID,
                            Type = TypeRecipeEnum.Manufacturing,
                            ItemId = item.productTypeID,
                            Quantity = item.quantity
                        };

                        if (Activity != null)
                            recipeDb.Time = Activity.time;

                        context.Recipes.Add(recipeDb);
                    }
                    context.SaveChanges();


                    //MAJ Materiaux recettes
                    context.Database.ExecuteSqlRaw("DELETE FROM RecipeRessources");
                    foreach (var item in ActivityMaterialsCsvrecords.Where(x => (TypeRecipeEnum)x.activityID == TypeRecipeEnum.Manufacturing && context.Recipes.Any(y => y.Id == x.typeID)))
                    {
                        var recipeRessources = new RecipeRessource()
                        {
                            RecipeId = item.typeID,
                            ItemId = item.materialTypeID,
                            Quantity = item.quantity
                        };

                        context.RecipeRessources.Add(recipeRessources);
                    }
                    context.SaveChanges();



                    industryActivityCsvrecords = null;
                    ActivityProductsCsvrecords = null;
                    industryActivityCsvrecords = null;
                    MarketGroupsCsvrecords = null;
                    invTypesCsvrecords = null;

                }
            }
            catch (Exception ex)
            {
                Logs.ClassLog.writeLog("InsertEsiSDE_Activity => Error= ");
                Logs.ClassLog.writeException(ex);
            }
        }

        private class IndustryActivityCsv
        {
            public int typeID { get; set; }
            public int activityID { get; set; }
            public int time { get; set; }
        }
        private class MarketGroupActivityCsv
        {
            public int marketGroupID { get; set; }
            public int? parentGroupID { get; set; }
            public string marketGroupName { get; set; }
            public string description { get; set; }
            public int? iconID { get; set; }
            public int hasTypes { get; set; }
        }

        private class ActivityMaterialsCsv
        {
            public int typeID { get; set; }
            public int activityID { get; set; }
            public int materialTypeID { get; set; }
            public int quantity { get; set; }
        }

        private class ActivityProductsCsv
        {
            public int typeID { get; set; }
            public int activityID { get; set; }
            public int productTypeID { get; set; }
            public int quantity { get; set; }
        }

        private class InvTypesCsv
        {
            public int typeID { get; set; }
            public int groupID { get; set; }
            public string typeName { get; set; }
            public string description { get; set; }
            public double mass { get; set; }
            public double volume { get; set; }
            public double capacity { get; set; }
            public int portionSize { get; set; }
            public int raceID { get; set; }
            public double basePrice { get; set; }
            public bool published { get; set; }
            public int marketGroupID { get; set; }
            public int iconID { get; set; }
            public int soundID { get; set; }
            public int graphicID { get; set; }
        }

        private class CustomInt32Converter : Int32Converter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                if (text == "N/A") 
                    return 0;
                if (text == "None") 
                    return 0;
                return base.ConvertFromString(text, row, memberMapData);
            }
        }
        private class MarketParetnInt32Converter : Int32Converter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                if (text == "N/A")
                    return null;
                if (text == "None")
                    return null;
                return base.ConvertFromString(text, row, memberMapData);
            }
        }

        private class CustomDoubleConverter : DoubleConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                if (text == "N/A") return 0.0;
                if (text == "None") return 0.0;
                return base.ConvertFromString(text, row, memberMapData);
            }
        }

        private class InvTypesCsvMap : ClassMap<InvTypesCsv>
        {
            public InvTypesCsvMap()
            {
                Map(m => m.typeID);
                Map(m => m.groupID);
                Map(m => m.typeName);
                Map(m => m.description);
                Map(m => m.mass);
                Map(m => m.volume);
                Map(m => m.capacity);
                Map(m => m.portionSize).TypeConverter<CustomInt32Converter>();
                Map(m => m.raceID).TypeConverter<CustomInt32Converter>();
                Map(m => m.basePrice).TypeConverter<CustomDoubleConverter>();
                Map(m => m.published);
                Map(m => m.marketGroupID).TypeConverter<CustomInt32Converter>();
                Map(m => m.iconID).TypeConverter<CustomInt32Converter>();
                Map(m => m.soundID).TypeConverter<CustomInt32Converter>();
                Map(m => m.graphicID).TypeConverter<CustomInt32Converter>();
            }

        }
        private class MarketGroupActivityCsvMap : ClassMap<MarketGroupActivityCsv>
        {
            public MarketGroupActivityCsvMap()
            {
                Map(m => m.marketGroupID);
                Map(m => m.parentGroupID).TypeConverter<MarketParetnInt32Converter>();
                Map(m => m.marketGroupName);
                Map(m => m.description);
                Map(m => m.iconID).TypeConverter<MarketParetnInt32Converter>();
                Map(m => m.hasTypes);
            }

        }
    }
}
