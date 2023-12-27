using Domain.BondAggreagte;
using Domain.BondAggreagte.Dto;

namespace Application.Calculation.Common.CalculationService.Dto;
public readonly record struct CalculationRequest(GetIncomeRequest Options, IEnumerable<Bond> Bonds);