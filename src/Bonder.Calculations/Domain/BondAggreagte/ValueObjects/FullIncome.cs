using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BondAggreagte.ValueObjects;
public readonly record struct FullIncome(StaticIncome StaticIncome, CouponIncome CouponIncome)
{
    public decimal FullIncomePercent => StaticIncome.NominalPercent + CouponIncome.CouponPercent;
}