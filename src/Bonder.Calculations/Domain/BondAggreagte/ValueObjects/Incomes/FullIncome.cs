namespace Domain.BondAggreagte.ValueObjects.Incomes;
public readonly record struct FullIncome(StaticIncome StaticIncome, CouponIncome CouponIncome, AmortizationIncome AmortizationIncome)
{
    public decimal FullIncomePercent => StaticIncome.PricePercent + CouponIncome.CouponPercent + AmortizationIncome.AmortizationPercent;
}