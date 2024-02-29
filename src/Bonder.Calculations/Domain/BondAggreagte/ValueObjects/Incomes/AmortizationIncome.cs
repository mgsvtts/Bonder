namespace Domain.BondAggreagte.ValueObjects.Incomes;

public readonly record struct AmortizationIncome(decimal AbsoluteAmortization, decimal AmortizationPercent)
{
    public static AmortizationIncome None => new AmortizationIncome(0, 0);
}