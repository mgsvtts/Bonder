using Application.Commands.Analyze.Dto;
using Domain.BondAggreagte.Abstractions;
using Domain.BondAggreagte.Abstractions.Dto;
using Domain.BondAggreagte.Dto;
using Mapster;
using Mediator;

namespace Application.Commands.Analyze;

public sealed class AnalyzeBondsCommandHandler : ICommandHandler<AnalyzeBondsCommand, Dictionary<BondWithIncome, IEnumerable<BondWithIncome>>>
{
    private const int _topFive = 5;

    private readonly IBondRepository _bondRepository;

    public AnalyzeBondsCommandHandler(IBondRepository bondRepository)
    {
        _bondRepository = bondRepository;
    }

    public async ValueTask<Dictionary<BondWithIncome, IEnumerable<BondWithIncome>>> Handle(AnalyzeBondsCommand request, CancellationToken cancellationToken)
    {
        var results = new Dictionary<BondWithIncome, IEnumerable<BondWithIncome>>();

        await HandleNotSpecialBonds(request, results, cancellationToken);

        await HandleSpecialBonds(request, results, cancellationToken);

        return results;
    }

    private async Task HandleNotSpecialBonds(AnalyzeBondsCommand request, Dictionary<BondWithIncome, IEnumerable<BondWithIncome>> results, CancellationToken cancellationToken)
    {
        var defaultIncomeRequest = GetIncomeRequest(request, null);

        var defaultBonds = await GetDefaultBonds(request, defaultIncomeRequest, cancellationToken);

        var notSpecialBonds = await _bondRepository.GetByTickersAsync(request.Bonds.Where(x => x.Option is null).Select(x => x.Id), cancellationToken);

        foreach (var notSpecialBond in notSpecialBonds)
        {
            results.Add((notSpecialBond, notSpecialBond.GetIncomeOnDate(defaultIncomeRequest)).Adapt<BondWithIncome>(),
                        defaultBonds);
        }
    }

    private async Task HandleSpecialBonds(AnalyzeBondsCommand request, Dictionary<BondWithIncome, IEnumerable<BondWithIncome>> results, CancellationToken cancellationToken)
    {
        var specialBonds = request.Bonds.Where(x => x.Option is not null);
        var loadedSpecialBonds = await _bondRepository.GetByTickersAsync(specialBonds.Select(x => x.Id), cancellationToken);

        foreach (var bondToAnalyze in specialBonds)
        {
            var incomeRequest = GetIncomeRequest(null, bondToAnalyze);

            var betterBonds = await GetBetterBondsAsync(request, bondToAnalyze, incomeRequest, cancellationToken);

            var bond = loadedSpecialBonds.First(x => x.Identity.Ticker == bondToAnalyze.Id);

            results.Add((bond, bond.GetIncomeOnDate(incomeRequest)).Adapt<BondWithIncome>(),
                        betterBonds);
        }
    }

    private async Task<List<BondWithIncome>> GetDefaultBonds(AnalyzeBondsCommand request, GetIncomeRequest defaultIncomeRequest, CancellationToken cancellationToken)
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
            token: cancellationToken
        );
        return SelectTop(defaultIncomeRequest, defaultBonds);
    }

    private async Task<List<BondWithIncome>> GetBetterBondsAsync(AnalyzeBondsCommand request, BondToAnalyze? bondToAnalyze, GetIncomeRequest incomeRequest, CancellationToken cancellationToken)
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
            token: cancellationToken
        );

        return SelectTop(incomeRequest, filteredBonds);
    }

    private static List<BondWithIncome> SelectTop(GetIncomeRequest incomeRequest, GetPriceSortedResponse bonds)
    {
        return bonds.Bonds
        .OrderByDescending(x => x.GetIncomeOnDate(incomeRequest).FullIncomePercent)
        .Take(_topFive)
        .Select(x => new BondWithIncome(x.Identity.Ticker,
                                        x.Name,
                                        x.Income.StaticIncome.AbsolutePrice,
                                        x.GetIncomeOnDate(incomeRequest).FullIncomePercent))
        .ToList();
    }

    private static GetIncomeRequest GetIncomeRequest(AnalyzeBondsCommand? request, BondToAnalyze? bondToAnalyze)
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