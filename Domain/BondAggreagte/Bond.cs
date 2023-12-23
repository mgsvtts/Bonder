using Domain.BondAggreagte.ValueObjects;

namespace Domain.BondAggreagte;

public class Bond
{
    private List<Coupon> _coupons = new List<Coupon>();

    public BondId Id { get; private set; }
    public string Name { get; private set; }
    public Money Money { get; private set; }
    public IReadOnlyList<Coupon> Coupons => _coupons.AsReadOnly();
    public DateTime MaturityDate { get; private set; }

    public static Bond Create(BondId id,
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

    public static Bond Create(BondId id,
                              string name,
                              Money money,
                              DateTime maturityDate,
                              Coupon coupon)
    {
        return new Bond
        {
            Id = id,
            Name = name.Trim(),
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
        return Coupons.Count * Coupons[0].Payout * (MaturityDate.Year - DateTime.Now.Year);
    }

    public override string ToString()
    {
        return Name;
    }
}