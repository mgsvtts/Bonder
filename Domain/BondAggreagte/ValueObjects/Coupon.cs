namespace Domain.BondAggreagte.ValueObjects;

public sealed class Coupon
{
    public DateTime Date { get; private set; }
    public decimal Payout { get; private set; }

    public Coupon(DateTime date, decimal payout)
    {
        Payout = payout;
        Date = date;
    }
}