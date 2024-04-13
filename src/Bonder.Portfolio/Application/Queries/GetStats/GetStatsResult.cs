using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.GetStats;
public readonly struct GetStatsResult
{
    public PercentItem Income { get; }
    public PercentItem Fee { get; }
    public PercentItem Commission { get; }
    public PercentItem BondIncome { get; }
    public PercentItem ShareIncome { get; }
    public PercentItem CouponIncome { get; }
    public PercentItem InputIncome { get; }
    public PercentItem SellIncome { get; }
    public PercentItem Tax { get; }

    public GetStatsResult(decimal fullPrice, 
                          decimal income,  
                          decimal fee,
                          decimal commission,
                          decimal bondIncome,
                          decimal shareIncome,
                          decimal couponIncome,
                          decimal inputIncome,
                          decimal sellIncome,
                          decimal tax)
    {
        Income = new PercentItem(income, fullPrice);
        Fee = new PercentItem(fee, fullPrice);
        Commission = new PercentItem(commission, fullPrice);
        BondIncome = new PercentItem(bondIncome, fullPrice);
        ShareIncome = new PercentItem(shareIncome, fullPrice);
        CouponIncome = new PercentItem(couponIncome, fullPrice);
        InputIncome = new PercentItem(inputIncome, fullPrice);
        SellIncome = new PercentItem(sellIncome, fullPrice);
        Tax = new PercentItem(tax, fullPrice);
    }

}

public readonly record struct PercentItem(decimal Amount, decimal Full)
{
    public decimal Percents => Amount / Full;
};