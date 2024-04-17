using Domain.BondAggreagte.ValueObjects.Identities;

namespace Application.Commands.Analyze.Dto;

public readonly record struct AdviceBondWithIncome(Ticker Id, string Name, decimal Price, decimal Income);