using Domain.BondAggreagte.Dto;
using Domain.BondAggreagte.Exceptions;
using Domain.BondAggreagte.ValueObjects;

namespace Domain.BondAggreagte;

public class Bond
{
    private List<Coupon> _coupons = new List<Coupon>();

    public Ticker Id { get; private set; }
    public string Name { get; private set; }
    public Money Money { get; private set; }
    public IReadOnlyList<Coupon> Coupons => _coupons.AsReadOnly();

    public Dates Dates { get; private set; }

    public static Bond Create(Ticker id,
                              string name,
                              Money money,
                              Dates dates,
                              IEnumerable<Coupon> coupons)
    {
        return new Bond
        {
            Id = id,
            Name = name,
            _coupons = coupons.ToList(),
            Money = money,
            Dates = dates
        };
    }

    public static Bond Create(Ticker id,
                              string name,
                              Money money,
                              Dates dates,
                              Coupon coupon)
    {
        return new Bond
        {
            Id = id,
            Name = name.Trim(),
            _coupons = new List<Coupon> { coupon },
            Money = money,
            Dates = dates
        };
    }

    public decimal GetFullIncome(GetIncomeRequest request)
    {
        return Money.NominalIncome + GetCouponOnlyIncome(request);
    }

    public decimal GetCouponOnlyIncome(GetIncomeRequest request)
    {
        var year = GetCalculationYear(request);

        if (year < DateTime.Now.Year)
        {
            throw new InvalidDateException();
        }

        return Coupons.Count * Coupons[0].Payout * (year - DateTime.Now.Year);
    }

    public override string ToString()
    {
        return Name;
    }

    private int GetCalculationYear(GetIncomeRequest request)
    {
        return request.Type switch
        {
            DateIntervalType.TillMaturityDate => Dates.MaturityDate.Year,
            DateIntervalType.TillOfferDate => (Dates.OfferDate != null ? Dates.OfferDate : Dates.MaturityDate).Value.Year,
            DateIntervalType.TillDate => request.Date.Value.Year,
            _ => throw new NotImplementedException(),
        };
    }
}