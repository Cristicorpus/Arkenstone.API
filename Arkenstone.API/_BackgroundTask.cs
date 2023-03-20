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

        Schedule(() => Arkenstone.Logic.Logs.ClassLog.purgelog()).ToRunNow();
        Schedule(() => TokenCheck.checkAllToken()).ToRunNow();
        Schedule(() => FuzzWorkDump.CheckDumpAsynctask()).ToRunNow();
        Schedule(() => CostIndiceDump.RefreshCostIndice()).ToRunNow();
        
        Schedule(() => RigsDump.CheckDumpAsynctask()).ToRunOnceIn(120).Seconds();
        Schedule(() => StructureDump.CheckDumpAsynctask()).ToRunOnceIn(120).Seconds();
        Schedule(() => MarketDump.RefreshMarketPrice()).ToRunOnceIn(120).Seconds();
        
        Schedule(() => AssetDump.ReloadAllItemsAsynctask()).ToRunOnceIn(300).Seconds();
        
        // ------------------------------------ CYCLIC TASK ----------------------------

        
        Schedule(() => AssetDump.ReloadAllItemsAsynctask()).ToRunEvery(3600).Seconds();
        Schedule(() => MarketDump.RefreshMarketPrice()).ToRunEvery(3600).Seconds();
        Schedule(() => CostIndiceDump.RefreshCostIndice()).ToRunEvery(3600).Seconds();

        // ------------------------------------ CYCLIC specific TASK ----------------------------
        
        Schedule(() => FuzzWorkDump.CheckDumpAsynctask()).ToRunEvery(1).Days().At(13, 0);
        Schedule(() => RigsDump.CheckDumpAsynctask()).ToRunEvery(1).Days().At(14, 0);
        Schedule(() => StructureDump.CheckDumpAsynctask()).ToRunEvery(1).Days().At(14, 0);
        Schedule(() => TokenCheck.checkAllToken()).ToRunEvery(3).Hours().At(30);
        Schedule(() => Arkenstone.Logic.Logs.ClassLog.purgelog()).ToRunEvery(1).Days().At(0, 30);

    }

}
