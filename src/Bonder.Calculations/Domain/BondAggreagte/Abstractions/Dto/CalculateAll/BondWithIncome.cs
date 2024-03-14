using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects.Incomes;

namespace Domain.BondAggreagte.Abstractions.Dto.CalculateAll;

public readonly record struct BondWithIncome(Bond Bond, FullIncome FullIncome);