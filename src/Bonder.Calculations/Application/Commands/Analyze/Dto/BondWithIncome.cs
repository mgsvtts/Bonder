using Domain.BondAggreagte.ValueObjects.Identities;

namespace Application.Commands.Analyze.Dto;

public readonly record struct BondWithIncome(Ticker Id, string Name, decimal Price, decimal Income);