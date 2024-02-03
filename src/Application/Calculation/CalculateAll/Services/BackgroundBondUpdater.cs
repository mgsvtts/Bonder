
using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application.Calculation.CalculateAll.Services;

public class BackgroundBondUpdater : BackgroundService
{
    private IDohodHttpClient _dohodHttpClient;
    private IBondRepository _bondRepository;
    private ITinkoffGrpcClient _grpcClient;

    private readonly IServiceScopeFactory _scopeFactory;

    public BackgroundBondUpdater(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!IsValidTime())
            {
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
                continue;
            }

            using var scope = InitServices();
            var couponTask = UpdateFloatingCoupons(stoppingToken);
            var ratingTask = UpdateRatingAsync(stoppingToken);

            await Task.WhenAll(couponTask, ratingTask);
        }
    }

    private async Task UpdateRatingAsync(CancellationToken token)
    {
        const int step = 50;
        var range = new Range(0, step);

        var count = await _bondRepository.CountAsync(token);
        while (range.End.Value < count)
        {
            try
            {
                await UpdateRatingAsync(range, token);
            }
            finally
            {
                range = new Range(range.End, range.End.Value + step);
            }
        }
    }

    private async Task UpdateRatingAsync(Range range, CancellationToken token)
    {
        var bonds = await _bondRepository.TakeRangeAsync(range, token);
        foreach (var bond in bonds)
        {
            var rating = await _dohodHttpClient.GetBondRatingAsync(bond.Identity.Isin, token);
            await _bondRepository.UpdateRating(bond.Identity, rating, token);
        }
    }

    private async Task UpdateFloatingCoupons(CancellationToken token)
    {
        var floatingBonds = await _bondRepository.GetAllFloatingAsync(token);
        foreach (var bond in floatingBonds)
        {
            var coupons = await _grpcClient.GetCouponsAsync(bond.Identity.InstrumentId, token);

            await _bondRepository.UpdateCoupons(coupons, bond.Identity, token);
        }
    }

    private static bool IsValidTime()
    {
        return DateTime.Now.Hour == 5;
    }

    private IServiceScope InitServices()
    {
        var scope = _scopeFactory.CreateScope();

        _bondRepository = scope.ServiceProvider.GetRequiredService<IBondRepository>();
        _dohodHttpClient = scope.ServiceProvider.GetRequiredService<IDohodHttpClient>();
        _grpcClient = scope.ServiceProvider.GetRequiredService<ITinkoffGrpcClient>();

        return scope;
    }
}