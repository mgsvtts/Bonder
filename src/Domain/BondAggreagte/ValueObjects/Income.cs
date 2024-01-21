namespace Domain.BondAggreagte.ValueObjects;
public readonly record struct Income
{
    public readonly decimal NominalIncomePercent { get; }
    public readonly decimal CouponIncomePercent { get; }

    public readonly decimal FullIncome => NominalIncomePercent + CouponIncomePercent;

    public Income(decimal couponIncome, decimal nominalIncome = 0)
    {
        NominalIncomePercent = nominalIncome;
        CouponIncomePercent = couponIncome;
    }
}
