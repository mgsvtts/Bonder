using Domain.BondAggreagte.ValueObjects;

namespace Domain.BondAggreagte;

public class Bond
{
    private List<Coupon> _coupons = new List<Coupon>();

    public string Id { get; private set; }
    public string Name { get; private set; }
    public Money Money { get; private set; }
    public IReadOnlyList<Coupon> Coupons => _coupons.AsReadOnly();
    public DateTime MaturityDate { get; private set; }

    public static Bond Create(string id,
                              string name,
                              Money money,
                              DateTime maturityDate,
                              IEnumerable<Coupon> coupons)
    {
        return new Bond
        {
            Id = id,
            Name = name,
            _coupons = coupons.ToList(),
            Money = money,
            MaturityDate = maturityDate
        };
    }

    public static Bond Create(string id,
                              string name,
                              Money money,
                              DateTime maturityDate,
                              Coupon coupon)
    {
        return new Bond
        {
            Id = id,
            Name = name,
            _coupons = new List<Coupon> { coupon },
            Money = money,
            MaturityDate = maturityDate
        };
    }

    public decimal GetFullIncome()
    {
        return Money.NominalIncome + GetCouponOnlyIncome();
    }

    public decimal GetCouponOnlyIncome()
    {
        decimal income = 0;

        foreach (var coupon in Coupons)
        {
            income += Coupons.Count * coupon.Payout * (MaturityDate.Year - DateTime.Now.Year);
        }

        return income;
    }

    public override string ToString()
    {
        return Name;
    }
}