using Domain.BondAggreagte;
using Domain.BondAggreagte.Dto;

namespace Application.Calculation.Common.CalculationService.Dto;
public readonly record struct CalculationRequest(GetIncomeRequest Options, IEnumerable<Bond> Bonds);

public readonly record struct SortedCalculationRequest(GetIncomeRequest Options,
                                                       List<Bond> PriceSortedBonds,
                                                       List<Bond> FullIncomeSortedBonds);
