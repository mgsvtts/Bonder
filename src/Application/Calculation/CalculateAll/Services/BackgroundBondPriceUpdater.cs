using System.Buffers.Text;
using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte;
using Domain.BondAggreagte.Repositories;
using Domain.BondAggreagte.ValueObjects;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application.Calculation.CalculateAll.Services;

public class BackgroundBondPriceUpdater : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    private IAllBondsReceiver _bondReceiver;
    private IBondRepository _bondRepository;
    private ITInkoffHttpClient _httpClient;

    public BackgroundBondPriceUpdater(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {

        while (!token.IsCancellationRequested)
        {
            try
            {
                await BeginAsync(token);
            }
            catch
            { }
        }
    }

    private async Task BeginAsync(CancellationToken token)
    {
        const int step = 10;
        var startRange = new Range(0, step);

        while (!token.IsCancellationRequested)
        {
            using var scope = InitServices();

            var bondsToUpdate = await TryReceiveAsync(startRange, token);

            RecreateRange(step, ref startRange);

            await ProcessBondsAsync(bondsToUpdate, token);
        }
    }

    private async Task ProcessBondsAsync(IEnumerable<KeyValuePair<Ticker, StaticIncome>> bondsToUpdate, CancellationToken token)
    {
        var notFoundTickers = await _bondRepository.UpdateIncomesAsync(bondsToUpdate, token);
        if (notFoundTickers.Count == 0)
        {
            return;
        }

        var notFoundBonds = await _httpClient.GetBondsByTickersAsync(notFoundTickers, token);

        await _bondRepository.AddAsync(notFoundBonds, token);
    }

    private IServiceScope InitServices()
    {
        var scope = _scopeFactory.CreateScope();

        _bondReceiver = scope.ServiceProvider.GetRequiredService<IAllBondsReceiver>();
        _bondRepository = scope.ServiceProvider.GetRequiredService<IBondRepository>();
        _httpClient = scope.ServiceProvider.GetRequiredService<ITInkoffHttpClient>();

        return scope;
    }

    private void RecreateRange(int step, ref Range startRange)
    {
        if (startRange.Start.Value > _bondReceiver.GetMaxRange())
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
            return await _bondReceiver.ReceiveAsync(startRange, token);
        }
        catch (RpcException)
        {
            return await TryRetryAsync(startRange, token);
        }
    }

    private async Task<IEnumerable<KeyValuePair<Ticker, StaticIncome>>> TryRetryAsync(Range start, CancellationToken token)
    {
        var retries = 5;
        while (retries > 0)
        {
            try
            {
                return await _bondReceiver.ReceiveAsync(start, token);
            }
            catch
            { }

            retries--;
        }

        return Array.Empty<KeyValuePair<Ticker, StaticIncome>>();
    }
}