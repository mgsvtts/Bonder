using Domain.BondAggreagte;

namespace Application.Calculation.Common.CalculationService.Dto;

public readonly record struct CalculationResult(Bond Bond,
                                                int Priority);