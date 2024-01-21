using Domain.BondAggreagte;

namespace Application.Calculation.Common.CalculationService.Dto;
public sealed record CalculationRatingResult(Bond Bond, int? Rating) : CalculationItem(Bond);
