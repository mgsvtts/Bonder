using Domain.BondAggreagte;

namespace Domain.BondAggreagte.Abstractions.Dto.CalculateAll;

public readonly record struct CalculationResult(Bond Bond,
                                                int Priority);