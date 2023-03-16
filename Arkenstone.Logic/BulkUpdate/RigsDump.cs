using Arkenstone.Entities.DbSet;
using Arkenstone.Entities;
using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Arkenstone.Logic.GlobalTools;
using Arkenstone.Logic.Esi;

namespace Arkenstone.Logic.BulkUpdate
{
    public static class RigsDump
    {
        //public delegate List<U> delegateCSV<T, U>(List<T> list);
        private static string _folderPathESI = "Applicatif/fuzzwork/";
        private static string _folderPathOrigineRigs = "Applicatif/Manual/";
        public static void CheckDumpAsynctask()
        {
            _ = CheckDump();
        }
        public static async Task CheckDump()
        {
            var datebegin = DateTime.Now;

            if (!Directory.Exists(_folderPathESI))
                Directory.CreateDirectory(_folderPathESI);

            if (NewRigsDump())
            {
                Logs.ClassLog.writeLog("NewRigsDump => " + DateTime.Now.Subtract(datebegin).ToString() + " Secondes");

                datebegin = DateTime.Now;
                InsertRigs_Activity();
                Logs.ClassLog.writeLog("InsertRigs_Activity => " + DateTime.Now.Subtract(datebegin).ToString() + " Secondes");

                datebegin = DateTime.Now;
                await UpdateRigs_Activity();
                Logs.ClassLog.writeLog("UpdateRigs_Activity => " + DateTime.Now.Subtract(datebegin).ToString() + " Secondes");
            }

        }

        private static bool NewRigsDump()
        {
            bool response = false;
            Logs.ClassLog.writeLog("NewRigsDump => Check if dump RIGS is different");
            List<string> allfildownload = new List<string>();

            allfildownload.Add("RigsManufacturings.csv");

            foreach (var item in allfildownload)
            {
                byte[] oldchecksum = { };
                byte[] newchecksum = { };


                //on calcul le checksum precedent si il exist
                if (File.Exists(_folderPathESI+item))
                {
                    using var md5old = MD5.Create();
                    using (var stream = File.OpenRead(_folderPathESI + item))
                    {
                        oldchecksum = md5old.ComputeHash(stream);
                    }
                }

                //on recupere le checksum du nouveau
                using var md5new = MD5.Create();
                using (var stream = File.OpenRead(_folderPathOrigineRigs + item))
                {
                    oldchecksum = md5new.ComputeHash(stream);
                }

                if (!newchecksum.SequenceEqual(oldchecksum))
                {
                    if (File.Exists(_folderPathESI + item))
                        File.Delete(_folderPathESI + item);

                    File.Copy(_folderPathOrigineRigs + item, _folderPathESI + item);
                    
                    response = true;
                }
            }

            if (response)
                Logs.ClassLog.writeLog("NewRigsDump => True");
            else
                Logs.ClassLog.writeLog("NewRigsDump => False");

            return response;
        }

        private static void InsertRigs_Activity()
        {

            Logs.ClassLog.writeLog("InsertRigs_Activity => Reinsertion des information RIGS dans les database");

            var _dbConnectionString = Environment.GetEnvironmentVariable("DB_DATA_connectionstring");
            var options = new DbContextOptionsBuilder<ArkenstoneContext>().UseMySql(_dbConnectionString, ServerVersion.AutoDetect(_dbConnectionString)).Options;
            using (ArkenstoneContext context = new ArkenstoneContext(options))
            {
                List<RigsManufacturingCsv> RigsManufacturingCsvrecords = CsvTools.ReadCsv<RigsManufacturingCsv>(_folderPathESI + "RigsManufacturings.csv");



                //on supprimes les recettes et les matériaux présent
                
                context.Database.ExecuteSqlRaw("DELETE FROM RigsManufacturings");
                
                //MAJ item
                foreach (var item in RigsManufacturingCsvrecords)
                {
                    try
                    {

                        context.RigsManufacturings.Add(
                           new RigsManufacturing()
                           {
                               Id = item.typeID,
                               Name = "",
                               MarketIdEffect = item.GroupMarketIdEffect,
                               MarketIdNotEffect = item.GroupMarketIdNotEffect
                           });
                    }
                    catch (Exception ex)
                    {

                    }
                }
                context.SaveChanges();

                RigsManufacturingCsvrecords = null;

            }
        }
        
        public static async Task UpdateRigs_Activity()
        {
            Logs.ClassLog.writeLog("UpdateRigs_Activity => Update des information des RIGS dans les database");

            var eveEsi = new EveEsiConnexion();
            var _dbConnectionString = Environment.GetEnvironmentVariable("DB_DATA_connectionstring");
            var options = new DbContextOptionsBuilder<ArkenstoneContext>().UseMySql(_dbConnectionString, ServerVersion.AutoDetect(_dbConnectionString)).Options;
            using (ArkenstoneContext context = new ArkenstoneContext(options))
            {
                foreach (var item in context.RigsManufacturings)
                {
                    var RigsItems = await eveEsi.EsiClient.Universe.Type(item.Id);


                    var MaterialEffect = RigsItems.Data.DogmaAttributes.Find(x => x.AttributeId == 2594);
                    var TimeEffect = RigsItems.Data.DogmaAttributes.Find(x => x.AttributeId == 2593);
                    var CostEffect = RigsItems.Data.DogmaAttributes.Find(x => x.AttributeId == 2595);

                    var MultiplierHS = RigsItems.Data.DogmaAttributes.Find(x => x.AttributeId == 2355);
                    var MultiplierLS = RigsItems.Data.DogmaAttributes.Find(x => x.AttributeId == 2356);
                    var MultiplierNS = RigsItems.Data.DogmaAttributes.Find(x => x.AttributeId == 2357);
                    var MultiplierValid = RigsItems.Data.DogmaAttributes.Find(x => x.AttributeId == 2358);

                    item.Name = RigsItems.Data.Name;

                    item.MaterialEffect = MaterialEffect != null ? (decimal)MaterialEffect.Value : 0;
                    item.TimeEffect = TimeEffect != null ? (decimal)TimeEffect.Value : 0;
                    item.CostEffect = CostEffect != null ? (decimal)CostEffect.Value : 0;

                    if (MultiplierValid != null && MultiplierValid.Value == 1)
                    {
                        item.MultiplierHS = MultiplierHS != null ? (decimal)MultiplierHS.Value : 1;
                        item.MultiplierLS = MultiplierLS != null ? (decimal)MultiplierLS.Value : 1;
                        item.MultiplierNS = MultiplierNS != null ? (decimal)MultiplierNS.Value : 1;
                    }
                    else
                    {
                        item.MultiplierHS = 1;
                        item.MultiplierLS = 1;
                        item.MultiplierNS = 1;
                    }
                }
                context.SaveChanges();
            }
        }


        private class RigsManufacturingCsv
        {
            public int typeID { get; set; }
            public string GroupMarketIdEffect { get; set; }
            public string GroupMarketIdNotEffect { get; set; }
        }
    }
}
