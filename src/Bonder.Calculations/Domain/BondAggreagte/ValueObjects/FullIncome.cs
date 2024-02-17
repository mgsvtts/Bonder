namespace Domain.BondAggreagte.ValueObjects;
public readonly record struct FullIncome(StaticIncome StaticIncome, CouponIncome CouponIncome)
{
    public decimal FullIncomePercent => StaticIncome.NominalPercent + CouponIncome.CouponPercent;
}