namespace Domain.BondAggreagte.ValueObjects;

public sealed class Coupon
{
    public DateTime PaymentDate { get; private set; }
    public decimal Payout { get; private set; }
    public DateTime DividendCutOffDate { get; private set; }
    public bool IsFloating { get; private set; }

    public Coupon(DateTime paymentDate, decimal payout, DateTime dividendCutOffDate, bool isFloating)
    {
        Payout = payout;
        PaymentDate = paymentDate.Date;
        DividendCutOffDate = dividendCutOffDate.Date;
        IsFloating = isFloating;
    }

    public bool CanGetCoupon(DateTime tillDate, bool considerDividendCutOffDate)
    {
        var paymentDate = considerDividendCutOffDate ? PaymentDate.Add(PaymentDate - DividendCutOffDate)
                                                     : PaymentDate;

        return tillDate.Date >= paymentDate && DateTime.Now.Date < paymentDate;
    }
}