using Arkenstone.Entities;
using Arkenstone.Logic.Esi;
using Arkenstone.Logic.GlobalTools;
using Arkenstone.Logic.Logs;
using ESI.NET;
using ESI.NET.Enumerations;
using ESI.NET.Models.Industry;
using ESI.NET.Models.Market;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Arkenstone.Logic.BulkUpdate.MarketDump;

namespace Arkenstone.Logic.BulkUpdate
{
    public static class CostIndiceDump
    {

        /// <summary>
        /// Refresh la table dataprice
        /// </summary>
        /// <param name="OreId"></param>
        public static void RefreshCostIndice()
        {
            try
            {
                ClassLog.writeLog("RefreshCostIndice => lancement des analyses de costIndice");
                EveEsiConnexion tmpEsiConnection = new EveEsiConnexion();

                var _dbConnectionString = Environment.GetEnvironmentVariable("DB_DATA_connectionstring");
                var options = new DbContextOptionsBuilder<ArkenstoneContext>().UseMySql(_dbConnectionString, ServerVersion.AutoDetect(_dbConnectionString)).Options;
                using (ArkenstoneContext context = new ArkenstoneContext(options))
                {

                    //recuperation de tout les datas
                    EsiResponse<List<SolarSystem>> resultquery = null;  
                    Retry.Do(() =>
                    {
                        resultquery = tmpEsiConnection.EsiClient.Industry.SolarSystemCostIndices().Result;
                        if (resultquery.StatusCode != HttpStatusCode.OK && resultquery.StatusCode != HttpStatusCode.NotFound)
                            throw new Exception();
                    }, TimeSpan.FromMilliseconds(50), 5);

                    foreach (var solarSystem in resultquery.Data)
                        updateCostIndices(context, solarSystem.SolarSystemId, solarSystem.CostIndices);
                    
                    context.SaveChanges();
                }
                ClassLog.writeLog("RefreshCostIndice => terminer");
            }
            catch (Exception ex)
            {
                ClassLog.writeLog("erreur on RefreshCostIndice global :/");
                ClassLog.writeException(ex);
            }
        }

        private static void updateCostIndices(ArkenstoneContext context, int solarsystem, List<CostIndice> costIndices)
        {
            var manufacture = context.CostIndices.FirstOrDefault(x => x.SolarSystemId == solarsystem && x.type == Entities.DbSet.CostIndiceType.manufacturing);
            var costIndice = costIndices.FirstOrDefault(x => x.Activity == "manufacturing");
            if (costIndice != null)
            {
                if (manufacture == null)
                    context.CostIndices.Add(new Entities.DbSet.CostIndice() { SolarSystemId = solarsystem, type = Entities.DbSet.CostIndiceType.manufacturing, Cost = costIndice.CostIndex });
                else
                    manufacture.Cost = costIndice.CostIndex;
            }

            manufacture = context.CostIndices.FirstOrDefault(x => x.SolarSystemId == solarsystem && x.type == Entities.DbSet.CostIndiceType.reaction);
            costIndice = costIndices.FirstOrDefault(x => x.Activity == "reaction");
            if (costIndice != null)
            {
                if (manufacture == null)
                    context.CostIndices.Add(new Entities.DbSet.CostIndice() { SolarSystemId = solarsystem, type = Entities.DbSet.CostIndiceType.reaction, Cost = costIndice.CostIndex });
                else
                    manufacture.Cost = costIndice.CostIndex;
            }
        }



    }
}
