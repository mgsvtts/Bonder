using Domain.BondAggreagte.ValueObjects.Identities;

namespace Application.Commands.Analyze.Dto;

public readonly record struct AnalyzeBondWithIncome(Ticker Id, string Name, decimal Price, decimal Income);