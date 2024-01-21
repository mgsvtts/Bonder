using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte;
using Domain.BondAggreagte.Repositories;
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
        IEnumerable<Bond> bondsToAdd = new Bond[step];

        while (!stoppingToken.IsCancellationRequested)
        {
            bondsToAdd = await TryReceiveAsync(startRange, bondsToAdd, stoppingToken);

            RecreateRange(step, ref startRange);

            await _bondRepository.AddOrUpateAsync(bondsToAdd, stoppingToken);
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

    private async Task<IEnumerable<Bond>> TryReceiveAsync(Range startRange, IEnumerable<Bond> bondsToAdd, CancellationToken stoppingToken)
    {
        try
        {
            bondsToAdd = await _bondReceiver.ReceiveAsync(startRange, stoppingToken);
        }
        catch (RpcException)
        {
            bondsToAdd = await TryRetryAsync(startRange, bondsToAdd, stoppingToken);
        }

        return bondsToAdd;
    }

    private async Task<IEnumerable<Bond>> TryRetryAsync(Range start, IEnumerable<Bond> bondsToUpdate, CancellationToken stoppingToken)
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

        return Array.Empty<Bond>();
    }
}
