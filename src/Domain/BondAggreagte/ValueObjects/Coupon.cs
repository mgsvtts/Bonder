namespace Domain.BondAggreagte.ValueObjects;

public readonly record struct Coupon(DateOnly PaymentDate, decimal Payout, DateOnly DividendCutOffDate, bool IsFloating)
{
    public bool CanGetCoupon(DateOnly tillDate, bool considerDividendCutOffDate)
    {
        var paymentDate = considerDividendCutOffDate ? PaymentDate.AddDays(PaymentDate.DayNumber - DividendCutOffDate.DayNumber)
                                                             : PaymentDate;

        return tillDate >= paymentDate && DateOnly.FromDateTime(DateTime.Now.Date) < paymentDate;
    }
}
