using Domain.BondAggreagte;
using Domain.BondAggreagte.Dto;
using Domain.BondAggreagte.ValueObjects;

namespace Application.Calculation.Common.CalculationService.Dto;
public readonly record struct CalculationRequest(GetIncomeRequest Options, IEnumerable<Bond> Bonds);