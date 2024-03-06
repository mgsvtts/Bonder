using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects.Incomes;

namespace Application.Calculation.Common.CalculationService.Dto;

public readonly record struct BondWithIncome(Bond Bond, FullIncome FullIncome);
