namespace Domain.BondAggreagte.ValueObjects.Incomes;

public readonly record struct Coupon(DateOnly PaymentDate, decimal Payout, DateOnly? DividendCutOffDate, bool IsFloating)
{
    public bool CanGetCoupon(DateOnly dateFrom, DateOnly dateTo, bool considerDividendCutOffDate)
    {
        var paymentDate = PaymentDate;
        if (considerDividendCutOffDate && DividendCutOffDate is not null)
        {
            paymentDate = PaymentDate.AddDays(PaymentDate.DayNumber - DividendCutOffDate.Value.DayNumber);
        }

        return paymentDate >= dateFrom && paymentDate <= dateTo;
    }
}