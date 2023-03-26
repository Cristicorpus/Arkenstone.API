using Arkenstone.Logic.Esi;
using Arkenstone.Logic.GlobalTools;
using Arkenstone.Logic.Logs;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using ESI.NET;
using ESI.NET.Models.Industry;
using System.Net;
using CostIndice = Arkenstone.Entities.DbSet.CostIndice;

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

                using (ArkenstoneContext context = ArkenstoneContext.GetContextWithDefaultOption())
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

        private static void updateCostIndices(ArkenstoneContext context, int solarsystem, List<ESI.NET.Models.Industry.CostIndice> costIndices)
        {
            var manufacture = context.CostIndices.FirstOrDefault(x => x.SolarSystemId == solarsystem && x.type == CostIndiceType.manufacturing);
            var costIndice = costIndices.FirstOrDefault(x => x.Activity == "manufacturing");
            if (costIndice != null)
            {
                if (manufacture == null)
                    context.CostIndices.Add(new CostIndice() { SolarSystemId = solarsystem, type = CostIndiceType.manufacturing, Cost = costIndice.CostIndex });
                else
                    manufacture.Cost = costIndice.CostIndex;
            }

            manufacture = context.CostIndices.FirstOrDefault(x => x.SolarSystemId == solarsystem && x.type == CostIndiceType.reaction);
            costIndice = costIndices.FirstOrDefault(x => x.Activity == "reaction");
            if (costIndice != null)
            {
                if (manufacture == null)
                    context.CostIndices.Add(new CostIndice() { SolarSystemId = solarsystem, type = CostIndiceType.reaction, Cost = costIndice.CostIndex });
                else
                    manufacture.Cost = costIndice.CostIndex;
            }
        }

    }
}
