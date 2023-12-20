using Domain.BondAggreagte.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BondAggreagte;
public class Bond
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public Money Money { get; private set; }
    public Coupon Coupon { get; private set; }
    public DateTime MaturityDate { get; private set; }

    public Bond(string id, string name, Coupon coupon, Money money, DateTime maturityDate)
    {
        Id = id;
        Name = name;
        Coupon = coupon;
        Money = money;
        MaturityDate = maturityDate;
    }

    public double GetFullIncome()
    {
        return Money.DenominationIncome + Coupon.YearPayout * (MaturityDate.Year - DateTime.Now.Year);
    }

    public double GetCouponOnlyIncome()
    {
        return Coupon.YearPayout * (MaturityDate.Year - DateTime.Now.Year);
    }

    public override string ToString()
    {
        return Name;
    }
}
