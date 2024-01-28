
using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte.Repositories;
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
        while (!stoppingToken.IsCancellationRequested && IsValidTime())
        {
            using var scope = InitServices();
            var floatingTask = UpdateFloatingCoupons(stoppingToken);
            var ratingTask = UpdateRatingAsync(stoppingToken);

            await Task.WhenAll(floatingTask, ratingTask);
        }
    }

    private async Task UpdateRatingAsync(CancellationToken token)
    {
        const int step = 50;
        var range = new Range(0, step);

        while (range.Start.Value < await _bondRepository.CountAsync(token))
        {
            var bonds = await _bondRepository.TakeRangeAsync(range, token);
            foreach (var bond in bonds)
            {
                var rating = await _dohodHttpClient.GetBondRatingAsync(bond.Identity.Isin, token);
                await _bondRepository.UpdateRating(bond.Identity, rating ?? 0, token);
            }
            range = new Range(range.End, step);
        }
    }

    private async Task UpdateFloatingCoupons(CancellationToken token)
    {
        var floatingBonds = await _bondRepository.GetAllFloatingAsync(token);
        foreach (var bond in floatingBonds)
        {
            var coupons = await _grpcClient.GetBondCouponsAsync(bond.Identity.Id, token);

            await _bondRepository.UpdateCoupons(coupons, bond.Identity, token);
        }
    }

    private static bool IsValidTime()
    {
        return DateTime.Now.Hour == 5 && DateTime.Now.Minute == 0;
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