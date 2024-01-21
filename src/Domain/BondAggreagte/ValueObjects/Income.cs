namespace Domain.BondAggreagte.ValueObjects;
public readonly record struct Income
{
    public readonly decimal NominalIncome { get; }
    public readonly decimal CouponIncome { get; }

    public readonly decimal FullIncome => NominalIncome + CouponIncome;

    public Income(decimal couponIncome, decimal nominalIncome = 0)
    {
        NominalIncome = nominalIncome;
        CouponIncome = couponIncome;
    }
}
