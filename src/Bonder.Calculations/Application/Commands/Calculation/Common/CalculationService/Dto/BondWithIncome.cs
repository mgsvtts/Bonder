using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects.Incomes;

namespace Application.Commands.Calculation.Common.CalculationService.Dto;

public readonly record struct BondWithIncome(Bond Bond, FullIncome FullIncome);