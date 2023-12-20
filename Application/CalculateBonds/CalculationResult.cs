using Domain.BondAggreagte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CalculateBonds;

public sealed record CalculationResult(IEnumerable<CalculatedBond> CalculatedBonds,
                                       IEnumerable<Bond> PriceSortedBonds,
                                       IEnumerable<Bond> CouponIncomeSortedBonds,
                                       IEnumerable<Bond> IncomeSortedBonds);
public sealed record CalculatedBond(Bond Bond, int Priority);