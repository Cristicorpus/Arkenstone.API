using FluentScheduler;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Arkenstone.Logic.Asset;
using Arkenstone.Logic.FuzzWork;

public class _BackgroundTask : Registry
{
    public _BackgroundTask()
    {
        // Schedule an ITask to run at an interval
        // Schedule<MyTask>().ToRunNow().AndEvery(2).Seconds();


        //Schedule(() => Arkenstone.Models.EVEESIFunction.checkAllScope()).ToRunNow().AndEvery(3).Hours().At(30);
        //Schedule(() => Arkenstone.Models.MarketAndData.PopulateDataPrice()).ToRunNow().AndEvery(3).Hours().At(50);

        //Schedule(() => SeatRefresh.RereshInventory()).ToRunNow().AndEvery(30).Minutes();

        Schedule(() => AssetDump.ReloadAllItemsAsynctask()).ToRunNow().AndEvery(AssetDump.ReloadSecondeSpan).Seconds();
        Schedule(() => FuzzWorkDumpDb.CheckDump()).ToRunNow().AndEvery(1).Days().At(13, 0);

        Schedule(() => Arkenstone.Logic.Logs.ClassLog.purgelog()).ToRunNow().AndEvery(1).Days().At(0, 30);

    }

}
