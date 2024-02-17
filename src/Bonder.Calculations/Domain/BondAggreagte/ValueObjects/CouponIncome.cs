namespace Domain.BondAggreagte.ValueObjects;
public readonly record struct CouponIncome(decimal AbsoluteCoupon, decimal CouponPercent)
{
    public static CouponIncome None => new CouponIncome(0, 0);
}