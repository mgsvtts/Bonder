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
        while (!stoppingToken.IsCancellationRequested)
        {
            AllBonds.State = (await _bondReceiver.ReceiveAsync(stoppingToken)).ToList();

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
