using Domain.BondAggreagte;

namespace Application.Commands.Calculation.Common.CalculationService.Dto;
public sealed record CalculationRatingResult(Bond Bond, int? Rating) : CalculationItem(Bond);