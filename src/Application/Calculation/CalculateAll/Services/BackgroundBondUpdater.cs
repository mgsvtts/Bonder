using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte;
using Domain.BondAggreagte.Repositories;
using Domain.BondAggreagte.ValueObjects;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application.Calculation.CalculateAll.Services;

public class BackgroundBondUpdater : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    private IAllBondsReceiver _bondReceiver;
    private IBondRepository _bondRepository;

    public BackgroundBondUpdater(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = InitServices();

        const int step = 10;
        var startRange = new Range(0, step);
        IEnumerable<KeyValuePair<Ticker, StaticIncome>> bondsToUpdate = new KeyValuePair<Ticker, StaticIncome>[step];

        while (!stoppingToken.IsCancellationRequested)
        {
            bondsToUpdate = await TryReceiveAsync(startRange, stoppingToken);

            RecreateRange(step, ref startRange);

            await _bondRepository.UpdateAsync(bondsToUpdate, stoppingToken);
        }
    }

    private IServiceScope InitServices()
    {
        var scope = _scopeFactory.CreateScope();

        _bondReceiver = scope.ServiceProvider.GetRequiredService<IAllBondsReceiver>();
        _bondRepository = scope.ServiceProvider.GetRequiredService<IBondRepository>();

        return scope;
    }

    private void RecreateRange(int step, ref Range startRange)
    {
        if (startRange.Start.Value > _bondReceiver.MaxRange)
        {
            startRange = new Range(0, step);
        }
        else
        {
            startRange = new Range(startRange.End, startRange.End.Value + step);
        }
    }

    private async Task<IEnumerable<KeyValuePair<Ticker, StaticIncome>>> TryReceiveAsync(Range startRange,  
                                                                                         CancellationToken token)
    {
        try
        {
            return  await _bondReceiver.ReceiveAsync(startRange, token);
        }
        catch (RpcException)
        {
            return await TryRetryAsync(startRange, token);
        }
    }

    private async Task<IEnumerable<KeyValuePair<Ticker, StaticIncome>>> TryRetryAsync(Range start, CancellationToken stoppingToken)
    {
        var retries = 5;
        while (retries > 0)
        {
            try
            {
                return await _bondReceiver.ReceiveAsync(start, stoppingToken);
            }
            catch
            { }

            retries--;
        }

        return Array.Empty<KeyValuePair<Ticker, StaticIncome>>();
    }
}
