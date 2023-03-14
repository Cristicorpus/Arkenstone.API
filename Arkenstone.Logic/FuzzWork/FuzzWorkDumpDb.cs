using CsvHelper;
using CsvHelper.TypeConversion;
using CsvHelper.Configuration;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using ESI.NET.Models.Corporation;

namespace Arkenstone.Logic.FuzzWork
{
    public class FuzzWorkDumpDb
    {
        public delegate List<U> delegateCSV<T, U>(List<T> list);
        private static string _folderPathESI = "Applicatif/fuzzwork/";
        public static void CheckDump()
        {
            var datebegin = DateTime.Now;

            if (!Directory.Exists(_folderPathESI))
                Directory.CreateDirectory(_folderPathESI);

            if (NewEsiDump())
            {
                Logs.ClassLog.writeLog("NewEsiDump => " + DateTime.Now.Subtract(datebegin).ToString() + " Secondes");
                InsertEsiSDE_Activity();
                Logs.ClassLog.writeLog("InsertEsiSDE_Activity => " + DateTime.Now.Subtract(datebegin).ToString() + " Secondes");
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

            Logs.ClassLog.writeLog("InsertEsiSDE => Reinsertion des information ESI dans les recettes");

            var _dbConnectionString = System.Environment.GetEnvironmentVariable("DB_DATA_connectionstring");
            var options = new DbContextOptionsBuilder<ArkenstoneContext>().UseMySql(_dbConnectionString, ServerVersion.AutoDetect(_dbConnectionString)).Options;
            using (ArkenstoneContext context = new ArkenstoneContext(options))
            {
                List<ActivityProductsCsv> ActivityProductsCsvrecords = ReadCsv<ActivityProductsCsv>(_folderPathESI + "industryActivityProducts.csv");
                List<ActivityMaterialsCsv> ActivityMaterialsCsvrecords = ReadCsv<ActivityMaterialsCsv>(_folderPathESI + "industryActivityMaterials.csv");
                List<IndustryActivityCsv> industryActivityCsvrecords = ReadCsv<IndustryActivityCsv>(_folderPathESI + "industryActivity.csv");
                List<InvTypesCsv> invTypesCsvrecords = ReadCsv<InvTypesCsv>(_folderPathESI + "invTypes.csv", new InvTypesCsvMap());



                //on supprimes les recettes et les matériaux présent

                //TODO:ne pas supprimer la table des item, essayer de plutot mettre a jours 
                context.Database.ExecuteSqlRaw("DELETE FROM Items");

                //ces deux table la peuvent etre supprimer :) et recrer completement
                context.Database.ExecuteSqlRaw("DELETE FROM RecipeRessources");
                context.Database.ExecuteSqlRaw("DELETE FROM Recipes");

                //MAJ item
                foreach (var item in invTypesCsvrecords)
                {
                    context.Items.Add(
                       new Item()
                       {
                           Id = item.typeID,
                           Name = item.typeName,
                           Published = item.published,
                           MarketGroupId = item.marketGroupID,
                           PriceBuy = 0,
                           PriceSell = 0,
                       });
                }
                context.SaveChanges();

                //MAJ recettes
                foreach (var item in ActivityProductsCsvrecords.Where(x => (TypeRecipeEnum)x.activityID == TypeRecipeEnum.Manufacturing && context.Items.Any(y => y.Published == true && y.Id == x.productTypeID)))
                {
                    context.Recipes.Add(new Recipe()
                    {
                        Id = item.typeID,
                        Type = TypeRecipeEnum.Manufacturing,
                        ItemId = item.productTypeID,
                        Quantity = item.quantity,
                        Time = industryActivityCsvrecords.Find(
                                x =>
                                {
                                    return (TypeRecipeEnum)x.activityID == TypeRecipeEnum.Manufacturing && x.typeID == item.typeID;
                                }).time
                    });
                }
                context.SaveChanges();

                //MAJ Materiaux recettes
                foreach (var item in ActivityMaterialsCsvrecords.Where(x => (TypeRecipeEnum)x.activityID == TypeRecipeEnum.Manufacturing && context.Recipes.Any(y => y.Id == x.typeID)))
                {
                    context.RecipeRessources.Add(new RecipeRessource()
                    {
                        RecipeId = item.typeID,
                        ItemId = item.materialTypeID,
                        Quantity = item.quantity
                    });
                }
                context.SaveChanges();

                industryActivityCsvrecords = null;
                ActivityProductsCsvrecords = null;
                industryActivityCsvrecords = null;
                invTypesCsvrecords = null;

            }
        }

        private static List<T> ReadCsv<T>(string path, ClassMap classMap = null)
        {
            List<T> csvrecords;
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
            {
                if (classMap != null)
                {
                    csv.Context.RegisterClassMap(classMap);
                }
                csvrecords = csv.GetRecords<T>().ToList();
            }
            return csvrecords;
        }

        private static int DeleteDontExist(List<InvTypesCsv> invTypes)
        {
            return invTypes.RemoveAll(invType => !invType.published);
        }

        private static int DeleteDontExist(List<ActivityProductsCsv> activityProducts, List<InvTypesCsv> invTypes)
        {
            return activityProducts.RemoveAll(product =>
            {
                //retirer les item qui n'existent pas
                if (invTypes.Exists(invType => invType.typeID == product.typeID) &&
                    invTypes.Exists(invType => invType.typeID == product.productTypeID))
                {
                    return false;
                }

                return true;
            });
        }

        private static int DeleteDontExist(List<ActivityMaterialsCsv> activityMaterials, List<ActivityProductsCsv> activityProducts, List<InvTypesCsv> invTypes)
        {
            return activityMaterials.RemoveAll(activityMaterial =>
            {
                //retirer les item qui n'existent pas
                if (activityProducts.Exists(x => x.typeID == activityMaterial.typeID) &&
                    invTypes.Exists(x => x.typeID == activityMaterial.materialTypeID))
                {
                    return false;
                }

                return true;
            });
        }

        private class IndustryActivityCsv
        {
            public int typeID { get; set; }
            public int activityID { get; set; }
            public int time { get; set; }
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
                if (text == "N/A") return 0;
                if (text == "None") return 0;
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
    }
}
