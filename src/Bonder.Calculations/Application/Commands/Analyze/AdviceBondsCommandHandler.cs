using Application.Commands.Analyze.Dto;
using Domain.BondAggreagte.Abstractions;
using Domain.BondAggreagte.Abstractions.Dto.GetPriceSorted;
using Domain.BondAggreagte.Dto;
using Mapster;
using Mediator;
using Shared.Domain.Common;
using System.Collections.Concurrent;

namespace Application.Commands.Analyze;

public sealed class AdviceBondsCommandHandler : ICommandHandler<AdviceBondsCommand, Dictionary<AdviceBondWithIncome, IEnumerable<AdviceBondWithIncome>>>
{
    private const int _topFive = 5;

    private readonly IBondRepository _bondRepository;

    public AdviceBondsCommandHandler(IBondRepository bondRepository)
    {
        _bondRepository = bondRepository;
    }

    public async ValueTask<Dictionary<AdviceBondWithIncome, IEnumerable<AdviceBondWithIncome>>> Handle(AdviceBondsCommand request, CancellationToken token)
    {
        var results = new ConcurrentDictionary<AdviceBondWithIncome, IEnumerable<AdviceBondWithIncome>>();

        var notSpecialTask = HandleNotSpecialBonds(request, results, token);

        var specialTask = HandleSpecialBonds(request, results, token);

        await Task.WhenAll(notSpecialTask, specialTask);

        return results.ToDictionary();
    }

    private async Task HandleNotSpecialBonds(AdviceBondsCommand request, ConcurrentDictionary<AdviceBondWithIncome, IEnumerable<AdviceBondWithIncome>> results, CancellationToken token)
    {
        var defaultIncomeRequest = GetIncomeRequest(request, null);

        var defaultBondsTask = GetDefaultBonds(request, defaultIncomeRequest, token);

        var notSpecialBondsTask = _bondRepository.GetByTickersAsync(request.Bonds.Where(x => x.Option is null).Select(x => x.Id), token);

        await Task.WhenAll(defaultBondsTask, notSpecialBondsTask);

        foreach (var notSpecialBond in notSpecialBondsTask.Result)
        {
            results.TryAdd((notSpecialBond, notSpecialBond.GetIncomeOnDate(defaultIncomeRequest)).Adapt<AdviceBondWithIncome>(),
                            defaultBondsTask.Result);
        }
    }

    private async Task HandleSpecialBonds(AdviceBondsCommand request, ConcurrentDictionary<AdviceBondWithIncome, IEnumerable<AdviceBondWithIncome>> results, CancellationToken token)
    {
        var specialBonds = request.Bonds.Where(x => x.Option is not null);
        var loadedSpecialBonds = await _bondRepository.GetByTickersAsync(specialBonds.Select(x => x.Id), token);

        foreach (var bondToAnalyze in specialBonds)
        {
            var incomeRequest = GetIncomeRequest(null, bondToAnalyze);

            var betterBonds = await GetBetterBondsAsync(request, bondToAnalyze, incomeRequest, token);

            var bond = loadedSpecialBonds.First(x => x.Identity.Ticker == bondToAnalyze.Id);

            results.TryAdd((bond, bond.GetIncomeOnDate(incomeRequest)).Adapt<AdviceBondWithIncome>(),
                            betterBonds);
        }
    }

    private async Task<List<AdviceBondWithIncome>> GetDefaultBonds(AdviceBondsCommand request, GetIncomeRequest defaultIncomeRequest, CancellationToken token)
    {
        var defaultBonds = await _bondRepository.GetPriceSortedAsync
        (
            new GetPriceSortedRequest
            (
                type: DateIntervalType.TillOfferDate,
                pageInfo: PageInfo.Default,
                priceFrom: request?.DefaultOptions?.PriceFrom ?? 0,
                priceTo: request?.DefaultOptions?.PriceTo ?? decimal.MaxValue,
                ratingFrom: request?.DefaultOptions?.RatingFrom ?? 0,
                ratingTo: request?.DefaultOptions?.RatingTo ?? 10,
                dateFrom: request?.DefaultOptions?.DateFrom,
                dateTo: request?.DefaultOptions?.DateTo
            ),
            takeAll: true,
            token: token
        );
        return SelectTop(defaultIncomeRequest, defaultBonds);
    }

    private async Task<List<AdviceBondWithIncome>> GetBetterBondsAsync(AdviceBondsCommand request, BondToAnalyze? bondToAnalyze, GetIncomeRequest incomeRequest, CancellationToken token)
    {
        var filteredBonds = await _bondRepository.GetPriceSortedAsync
        (
            new GetPriceSortedRequest
            (
                type: DateIntervalType.TillOfferDate,
                pageInfo: PageInfo.Default,
                priceFrom: bondToAnalyze?.Option?.PriceFrom ?? request?.DefaultOptions?.PriceFrom ?? 0,
                priceTo: bondToAnalyze?.Option?.PriceTo ?? request?.DefaultOptions?.PriceTo ?? decimal.MaxValue,
                ratingFrom: bondToAnalyze?.Option?.RatingFrom ?? request?.DefaultOptions?.RatingFrom ?? 0,
                ratingTo: bondToAnalyze?.Option?.RatingTo ?? request?.DefaultOptions?.RatingTo ?? 10,
                dateFrom: bondToAnalyze?.Option?.DateFrom ?? request?.DefaultOptions?.DateFrom,
                dateTo: bondToAnalyze?.Option?.DateTo ?? request?.DefaultOptions?.DateTo
            ),
            takeAll: true,
            token: token
        );

        return SelectTop(incomeRequest, filteredBonds);
    }

    private static List<AdviceBondWithIncome> SelectTop(GetIncomeRequest incomeRequest, GetPriceSortedResponse bonds)
    {
        return bonds.Bonds
        .OrderByDescending(x => x.GetIncomeOnDate(incomeRequest).FullIncomePercent)
        .Take(_topFive)
        .Select(x => new AdviceBondWithIncome(x.Identity.Ticker,
                                        x.Name.ToString(),
                                        x.Income.StaticIncome.AbsolutePrice,
                                        x.GetIncomeOnDate(incomeRequest).FullIncomePercent))
        .ToList();
    }

    private static GetIncomeRequest GetIncomeRequest(AdviceBondsCommand? request, BondToAnalyze? bondToAnalyze)
    {
        GetIncomeRequest incomeRequest;
        if (bondToAnalyze?.Option?.DateTo is not null)
        {
            incomeRequest = new GetIncomeRequest(DateIntervalType.TillCustomDate, dateTo: bondToAnalyze.Option.DateTo.Value);
        }
        else if (request?.DefaultOptions?.DateTo is not null)
        {
            incomeRequest = new GetIncomeRequest(DateIntervalType.TillCustomDate, dateTo: request.DefaultOptions.DateTo.Value);
        }
        else
        {
            incomeRequest = new GetIncomeRequest(DateIntervalType.TillOfferDate);
        }

        return incomeRequest;
    }
}