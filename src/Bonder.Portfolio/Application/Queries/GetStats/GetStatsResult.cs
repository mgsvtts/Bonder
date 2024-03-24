using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.GetStats;
public readonly record struct GetStatsResult(PercentItem Income,
                                             PercentItem Fee,
                                             PercentItem Commission,
                                             PercentItem BondIncome,
                                             PercentItem ShareIncome,
                                             PercentItem CouponIncome,
                                             PercentItem InputIncome,
                                             PercentItem SellIncome,
                                             PercentItem Tax);

public readonly record struct PercentItem(decimal Amount, decimal Full)
{
    public decimal Percents => Amount / Full;
};