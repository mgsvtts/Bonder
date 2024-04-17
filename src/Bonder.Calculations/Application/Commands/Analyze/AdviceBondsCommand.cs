using Application.Commands.Analyze.Dto;
using Domain.BondAggreagte.ValueObjects.Identities;
using Mediator;

namespace Application.Commands.Analyze;

public sealed record AdviceBondsCommand(AdviceOptions DefaultOptions, IEnumerable<BondToAnalyze> Bonds) : ICommand<Dictionary<AdviceBondWithIncome, IEnumerable<AdviceBondWithIncome>>>;

public sealed record BondToAnalyze(AdviceOptions? Option, Ticker Id);

public sealed record AdviceOptions(decimal? PriceFrom, decimal? PriceTo, int? RatingFrom, int? RatingTo, DateOnly? DateFrom, DateOnly? DateTo);