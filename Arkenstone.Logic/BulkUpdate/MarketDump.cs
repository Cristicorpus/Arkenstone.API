using Arkenstone.Logic.Entities;
using Arkenstone.Logic.Esi;
using Arkenstone.Logic.GlobalTools;
using Arkenstone.Logic.Logs;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using ESI.NET;
using ESI.NET.Enumerations;
using ESI.NET.Models.Market;
using System.Net;

namespace Arkenstone.Logic.BulkUpdate
{
    public static class MarketDump
    {

        /// <summary>
        /// Refresh la table dataprice
        /// </summary>
        /// <param name="OreId"></param>
        public static void RefreshMarketPrice()
        {
            try
            {
                ClassLog.writeLog("RefreshMarketPrice => lancement des analyses de market");

                using (ArkenstoneContext context = ArkenstoneContext.GetContextWithDefaultOption())
                {
                    ItemRepository itemRepository = new ItemRepository(context);
                    
                    var allUpdatableItem = itemRepository.GetAllItemWithPriceUpdatable();
                    int lenght = allUpdatableItem.Count();

                    ClassLog.writeLog("RefreshMarketPrice => " + lenght + " allItemDb à analyser");

                    //Update PriceSell and Buy
                    var taskList = new List<Task>();
                    int step = 500;
                    for (int i = 0; i < lenght; i+=step)
                    {
                        var tmp = allUpdatableItem.Skip(i).Take(step).ToList();
                        taskList.Add(new Task(() =>{TaskUpdate(tmp);}));
                    }
                    taskList.ForEach(x => x.Start());
                    Task.WaitAll(taskList.ToArray());

                    //Update AdjustedPrice
                    EveEsiConnexion tmpEsiConnection = new EveEsiConnexion();
                    tmpEsiConnection.EsiClient.Market.Prices().Result.Data.ToList().ForEach(x =>
                    {
                        var tmpitem = allUpdatableItem.FirstOrDefault(y => y.Id == x.TypeId);
                        if (tmpitem != null)
                            tmpitem.PriceAdjustedPrice = x.AdjustedPrice;
                    });
                    context.SaveChanges();
                }
                ClassLog.writeLog("RefreshMarketPrice => terminer");
            }
            catch (Exception ex)
            {
                ClassLog.writeLog("erreur on RefreshMarketPrice global :/");
                ClassLog.writeException(ex);
            }
        }


        public static void TaskUpdate(List<Item> Items)
        {
            EveEsiConnexion tmpEsiConnection = new EveEsiConnexion();
            foreach (var DataPriceItem in Items)
            {
                try
                {
                    List<orderPercentileProcessing> listofallorder = GetAllorder(tmpEsiConnection, 10000002, MarketOrderType.All, DataPriceItem.Id);

                    if (listofallorder.Any(x => !x.isbuyorder))
                        DataPriceItem.PriceSell = CalculatePercentile(95, listofallorder.OrderByDescending(x => x.value).Where(x => !x.isbuyorder).ToList());
                    if (listofallorder.Any(x => x.isbuyorder))
                        DataPriceItem.PriceBuy = CalculatePercentile(95, listofallorder.OrderBy(x => x.value).Where(x => x.isbuyorder).ToList());
                }
                catch (Exception ex)
                {
                    ClassLog.writeLog("erreur on :" + DataPriceItem.Id + " ," + DataPriceItem.Name);
                    ClassLog.writeException(ex);
                }
            }
            tmpEsiConnection = null;
        }


        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="RegionID"></param>
        /// <param name="sellorbuy"></param>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public static List<orderPercentileProcessing> GetAllorder(EveEsiConnexion tmpEsiConnection, int RegionID, MarketOrderType sellorbuy, int typeID)
        {

            //recuperation de tout les datas
            List<orderPercentileProcessing> array2 = new List<orderPercentileProcessing>();
            EsiResponse<List<Order>> resultquery = null;
            int page = 1;
            do
            {
                Retry.Do(() =>
                {
                    resultquery = tmpEsiConnection.EsiClient.Market.RegionOrders(RegionID, sellorbuy, page, typeID).Result;
                    if (resultquery.StatusCode != HttpStatusCode.OK && resultquery.StatusCode != HttpStatusCode.NotFound)
                        throw new Exception();
                }, TimeSpan.FromMilliseconds(50), 5);

                if (resultquery.StatusCode == HttpStatusCode.OK)
                {
                    foreach (Order order in resultquery.Data)
                        array2.Add(new orderPercentileProcessing() { value = order.Price, volume = order.VolumeRemain, isbuyorder = order.IsBuyOrder });
                }
                page++;
            } while (resultquery.Data.Count()>=1000 && resultquery.StatusCode == HttpStatusCode.OK);

            return array2;

        }

        /// <summary>
        /// pernet le calcul du percentil
        /// </summary>
        /// <param name="percentileSetpoint"></param>
        /// <param name="arrayOrder"></param>
        /// <returns></returns>
        public static decimal CalculatePercentile(int percentileSetpoint, List<orderPercentileProcessing> arrayOrder)
        {

            long volume_global = 0;
            long volume_percentil = 0;
            long count = 0;
            decimal resultpercentil = 0;

            if (arrayOrder == null)
                return 0;

            arrayOrder.ForEach(x => volume_global += x.volume);
            volume_percentil = (long)((double)volume_global * (double)((100.0 - percentileSetpoint) / 100.0));

            foreach (orderPercentileProcessing item in arrayOrder)
            {
                if (count < volume_percentil || ((count + item.volume) >= volume_percentil))
                {
                    resultpercentil = (decimal)item.value;
                }
                count += item.volume;
            }
            return resultpercentil;
        }

        /// <summary>
        /// 
        /// </summary>
        public class orderPercentileProcessing
        {
            public decimal value { get; set; }
            public long volume { get; set; }
            public bool isbuyorder { get; set; }
        }
    }
}
