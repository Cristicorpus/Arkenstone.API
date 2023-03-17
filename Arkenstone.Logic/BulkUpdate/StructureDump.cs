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
    public static class StructureDump
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

            if (NewStructuresDump())
            {
                Logs.ClassLog.writeLog("NewStructuresDump => " + DateTime.Now.Subtract(datebegin).ToString() + " Secondes");

                datebegin = DateTime.Now;
                InsertStructures_Activity();
                Logs.ClassLog.writeLog("InsertStructures_Activity => " + DateTime.Now.Subtract(datebegin).ToString() + " Secondes");

                datebegin = DateTime.Now;
                await UpdateStructures_Activity();
                Logs.ClassLog.writeLog("UpdateStructures_Activity => " + DateTime.Now.Subtract(datebegin).ToString() + " Secondes");
            }

        }

        private static bool NewStructuresDump()
        {
            bool response = false;
            Logs.ClassLog.writeLog("NewStructuresDump => Check if dump RIGS is different");
            List<string> allfildownload = new List<string>();

            allfildownload.Add("StructureTypes.csv");

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
                Logs.ClassLog.writeLog("NewStructuresDump => True");
            else
                Logs.ClassLog.writeLog("NewStructuresDump => False");

            return response;
        }

        private static void InsertStructures_Activity()
        {

            Logs.ClassLog.writeLog("InsertStructures_Activity => Reinsertion des information RIGS dans les database");

            var _dbConnectionString = Environment.GetEnvironmentVariable("DB_DATA_connectionstring");
            var options = new DbContextOptionsBuilder<ArkenstoneContext>().UseMySql(_dbConnectionString, ServerVersion.AutoDetect(_dbConnectionString)).Options;
            using (ArkenstoneContext context = new ArkenstoneContext(options))
            {
                List<StructuresTypesCsv> StructureTypesCsvrecords = CsvTools.ReadCsv<StructuresTypesCsv>(_folderPathESI + "StructureTypes.csv");

                //on supprimes les recettes et les matériaux présent

                context.Database.ExecuteSqlRaw("DELETE FROM StructureTypes");

                //MAJ item
                foreach (var item in StructureTypesCsvrecords)
                {
                    try
                    {

                        context.StructureTypes.Add(
                            new StructureType()
                            {
                                ItemId = item.typeID
                            });
                    }
                    catch (Exception ex)
                    {

                    }
                }
                context.SaveChanges();

                StructureTypesCsvrecords = null;

            }
        }
        
        public static async Task UpdateStructures_Activity()
        {
            Logs.ClassLog.writeLog("UpdateStructures_Activity => Update des information des RIGS dans les database");

            var eveEsi = new EveEsiConnexion();
            var _dbConnectionString = Environment.GetEnvironmentVariable("DB_DATA_connectionstring");
            var options = new DbContextOptionsBuilder<ArkenstoneContext>().UseMySql(_dbConnectionString, ServerVersion.AutoDetect(_dbConnectionString)).Options;
            using (ArkenstoneContext context = new ArkenstoneContext(options))
            {
                foreach (var item in context.StructureTypes)
                {
                    var StructureItems = await eveEsi.EsiClient.Universe.Type(item.ItemId);


                    var MaterialEffect = StructureItems.Data.DogmaAttributes.Find(x => x.AttributeId == 2600);
                    var TimeEffect = StructureItems.Data.DogmaAttributes.Find(x => x.AttributeId == 2602);
                    var CostEffect = StructureItems.Data.DogmaAttributes.Find(x => x.AttributeId == 2601);

                    item.MaterialEffect = MaterialEffect != null ? (decimal)MaterialEffect.Value : 0;
                    item.TimeEffect = TimeEffect != null ? (decimal)TimeEffect.Value : 0;
                    item.CostEffect = CostEffect != null ? (decimal)CostEffect.Value : 0;

                }
                context.SaveChanges();
            }
        }


        private class StructuresTypesCsv
        {
            public int typeID { get; set; }
        }
    }
}
