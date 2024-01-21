using Domain.BondAggreagte;

namespace Application.Calculation.Common.CalculationService.Dto;
public sealed record CalculationMoneyResult(Bond Bond, decimal Money) : CalculationItem(Bond);
