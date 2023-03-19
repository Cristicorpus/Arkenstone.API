using FluentScheduler;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Arkenstone.Logic.Asset;
using Arkenstone.Logic.BulkUpdate;

public class _BackgroundTask : Registry
{
    public _BackgroundTask()
    {
        // Schedule an ITask to run at an interval
        // Schedule<MyTask>().ToRunNow().AndEvery(2).Seconds();


        //Schedule(() => Arkenstone.Models.EVEESIFunction.checkAllScope()).ToRunNow().AndEvery(3).Hours().At(30);
        //Schedule(() => Arkenstone.Models.MarketAndData.PopulateDataPrice()).ToRunNow().AndEvery(3).Hours().At(50);

        //Schedule(() => SeatRefresh.RereshInventory()).ToRunNow().AndEvery(30).Minutes();

        Schedule(() => AssetDump.ReloadAllItemsAsynctask()).ToRunEvery(3600).Seconds();
        Schedule(() => MarketDump.RefreshMarketPrice()).ToRunEvery(3600).Seconds();
        Schedule(() => FuzzWorkDump.CheckDumpAsynctask()).ToRunEvery(1).Days().At(13, 0);
        Schedule(() => RigsDump.CheckDumpAsynctask()).ToRunEvery(1).Days().At(14, 0);
        Schedule(() => StructureDump.CheckDumpAsynctask()).ToRunEvery(1).Days().At(14, 0);

        Schedule(() => FuzzWorkDump.CheckDumpAsynctask()).ToRunOnceIn(1).Seconds();
        Schedule(() => RigsDump.CheckDumpAsynctask()).ToRunOnceIn(120).Seconds();
        Schedule(() => StructureDump.CheckDumpAsynctask()).ToRunOnceIn(120).Seconds();
        Schedule(() => AssetDump.ReloadAllItemsAsynctask()).ToRunOnceIn(300).Seconds();
        
        Schedule(() => MarketDump.RefreshMarketPrice()).ToRunOnceIn(1).Seconds();




        Schedule(() => Arkenstone.Logic.Logs.ClassLog.purgelog()).ToRunNow().AndEvery(1).Days().At(0, 30);

    }

}
