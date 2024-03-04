using Application.Analyze.Dto;
using Domain.BondAggreagte.ValueObjects.Identities;
using Mediator;

namespace Application.Analyze;

public sealed record AnalyzeBondsCommand(AnalyzeOptions DefaultOptions, IEnumerable<BondToAnalyze> Bonds) : ICommand<Dictionary<BondWithIncome, IEnumerable<BondWithIncome>>>;

public sealed record BondToAnalyze(AnalyzeOptions? Option, Ticker Id);

public sealed record AnalyzeOptions(decimal? PriceFrom, decimal? PriceTo, int? RatingFrom, int? RatingTo, DateOnly? DateFrom, DateOnly? DateTo);