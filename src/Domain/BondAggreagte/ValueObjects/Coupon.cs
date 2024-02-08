namespace Domain.BondAggreagte.ValueObjects;

public readonly record struct Coupon(DateOnly PaymentDate, decimal Payout, DateOnly DividendCutOffDate, bool IsFloating)
{
    public bool CanGetCoupon(DateOnly dateFrom, DateOnly dateTo, bool considerDividendCutOffDate)
    {
        var paymentDate = considerDividendCutOffDate ? dateTo.AddDays(PaymentDate.DayNumber - DividendCutOffDate.DayNumber)
                                                             : dateTo;

        return dateFrom >= paymentDate && DateOnly.FromDateTime(DateTime.Now.Date) < paymentDate;
    }
}
