using Domain.BondAggreagte;

namespace Application.Calculation.Common.CalculationService;

public sealed record CalculationResult(IEnumerable<CalculatedBond> CalculatedBonds,
                                       IEnumerable<Bond> PriceSortedBonds,
                                       IEnumerable<Bond> CouponIncomeSortedBonds,
                                       IEnumerable<Bond> IncomeSortedBonds);
public sealed record CalculatedBond(Bond Bond, int Priority);