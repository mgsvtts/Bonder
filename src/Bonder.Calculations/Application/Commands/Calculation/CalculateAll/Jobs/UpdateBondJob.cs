using Application.Commands.Calculation.Common.Abstractions;
using Domain.BondAggreagte.Abstractions;
using Quartz;

namespace Application.Commands.Calculation.CalculateAll.Jobs;

[DisallowConcurrentExecution]
public sealed class BackgroundBondUpdater : IJob
{
    private readonly IDohodHttpClient _dohodHttpClient;
    private readonly IBondRepository _bondRepository;
    private readonly ITinkoffGrpcClient _grpcClient;

    public BackgroundBondUpdater(IDohodHttpClient dohodHttpClient,
                                 IBondRepository bondRepository,
                                 ITinkoffGrpcClient grpcClient)
    {
        _dohodHttpClient = dohodHttpClient;
        _bondRepository = bondRepository;
        _grpcClient = grpcClient;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var couponTask = UpdateFloatingCoupons(context.CancellationToken);
        var ratingTask = UpdateRatingAsync(context.CancellationToken);

        await Task.WhenAll(couponTask, ratingTask);
    }

    private async Task UpdateRatingAsync(CancellationToken token)
    {
        const int step = 50;
        var range = new Range(0, step);

        var count = await _bondRepository.CountAsync(token);
        while (range.Start.Value < count)
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

            bond.UpdateRating(rating);

            await _bondRepository.UpdateAsync(bond, token);
        }
    }

    private async Task UpdateFloatingCoupons(CancellationToken token)
    {
        var floatingBonds = await _bondRepository.GetAllFloatingAsync(token);
        foreach (var bond in floatingBonds)
        {
            var coupons = await _grpcClient.GetCouponsAsync(bond.Identity.InstrumentId, token);

            bond.UpdateCoupons(coupons);

            await _bondRepository.UpdateAsync(bond, token);
        }
    }
}