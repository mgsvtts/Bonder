namespace Domain.BondAggreagte.ValueObjects.Incomes;
public readonly record struct Amortization(DateOnly PaymentDate, decimal Payout);