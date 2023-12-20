using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BondAggreagte.ValueObjects;
public sealed class Coupon
{
    public DateTime Date { get; private set; }
    public double YearPayout { get; private set; }

    public Coupon(DateTime date, double payout, int payRate)
    {
        YearPayout = payout * payRate;
        Date = date;
    }
}