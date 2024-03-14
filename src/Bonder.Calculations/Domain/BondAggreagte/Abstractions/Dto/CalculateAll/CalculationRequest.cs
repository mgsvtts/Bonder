using Domain.BondAggreagte;
using Domain.BondAggreagte.Dto;

namespace Domain.BondAggreagte.Abstractions.Dto.CalculateAll;
public readonly record struct CalculationRequest(GetIncomeRequest Options, IEnumerable<Bond> Bonds);

public readonly record struct SortedCalculationRequest(GetIncomeRequest Options,
                                                       List<Bond> PriceSortedBonds,
                                                       List<Bond> FullIncomeSortedBonds);