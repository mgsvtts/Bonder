namespace Domain.BondAggreagte.Abstractions.Dto.CalculateAll;
public sealed record CalculationRatingResult(Bond Bond, int? Rating) : CalculationItem(Bond);