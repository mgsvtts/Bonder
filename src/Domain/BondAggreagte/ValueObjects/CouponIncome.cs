using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BondAggreagte.ValueObjects;
public readonly record struct CouponIncome(decimal AbsoluteCoupon, decimal CouponPercent)
{
    public static CouponIncome None => new CouponIncome(0, 0);
}