using Domain.BondAggreagte;

namespace Application.Commands.Calculation.Common.CalculationService.Dto;
public sealed record CalculationMoneyResult(Bond Bond, decimal Money) : CalculationItem(Bond);