namespace Domain.BondAggreagte.Abstractions.Dto.CalculateAll;
public sealed record CalculationMoneyResult(Bond Bond, decimal Money) : CalculationItem(Bond);