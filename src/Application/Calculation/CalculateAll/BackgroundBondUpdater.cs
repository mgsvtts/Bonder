using Application.Calculation.Common.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Calculation.CalculateAll;
public class BackgroundBondUpdater : BackgroundService
{
    private readonly IAllBondsReceiver _bondReceiver;

    public BackgroundBondUpdater(IAllBondsReceiver bondReceiver)
    {
        _bondReceiver = bondReceiver;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const int step = 10;
        var start = new Range(0, step);
        while (!stoppingToken.IsCancellationRequested)
        {
            AllBonds.State.AddRange(await _bondReceiver.ReceiveAsync(start, stoppingToken));

            start = new Range(start.End, start.End.Value + step);
        }
    }
}
