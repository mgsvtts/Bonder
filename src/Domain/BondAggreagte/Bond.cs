using Domain.BondAggreagte.Dto;
using Domain.BondAggreagte.Exceptions;
using Domain.BondAggreagte.ValueObjects;
using Domain.Common.Models;

namespace Domain.BondAggreagte;

public class Bond : AggregateRoot<BondId>
{
    private List<Coupon> _coupons = new List<Coupon>();

    public string Name { get; private set; }
    public Money Money { get; private set; }
    public Dates Dates { get; private set; }
    public int? Rating { get; private set; }
    public IReadOnlyList<Coupon> Coupons => _coupons.AsReadOnly();

    private Bond(BondId id) : base(id)
    { }

    public static Bond Create(BondId id,
                              string name,
                              Money money,
                              Dates dates,
                              int? rating,
                              IEnumerable<Coupon> coupons)
    {
        return new Bond(id)
        {
            Name = name,
            _coupons = coupons.ToList(),
            Money = money,
            Dates = dates,
            Rating = rating
        };
    }

    public Income GetIncome(GetIncomeRequest request)
    {
        var couponIncome = GetCouponOnlyIncome(request);

        if (IsFullIncomeDate(request))
        {
            return new Income(couponIncome, Money.NominalIncome);
        }

        return new Income(couponIncome);
    }

    private decimal GetCouponOnlyIncome(GetIncomeRequest request)
    {
        var date = GetTillDate(request);

        if (date < DateTime.Now)
        {
            throw new InvalidPaymentDateException(date);
        }

        var futureCoupons = Coupons.Where(x => x.CanGetCoupon(date, request.ConsiderDividendCutOffDate));

        if (Coupons.Any(x => x.IsFloating))
        {
            var latestCoupon = futureCoupons.Where(x => x.Payout != 0)
                                            .OrderByDescending(x => x.PaymentDate)
                                            .First();
            return latestCoupon.Payout * futureCoupons.Count();
        }
        else
        {
            return futureCoupons.Count() * Coupons[0].Payout;
        }
    }

    public override string ToString()
    {
        return Name;
    }

    private DateTime GetTillDate(GetIncomeRequest request)
    {
        return request.Type switch
        {
            DateIntervalType.TillMaturityDate => Dates.MaturityDate,
            DateIntervalType.TillOfferDate => (Dates.OfferDate != null ? Dates.OfferDate : Dates.MaturityDate).Value,
            DateIntervalType.TillDate => request.TillDate.Value,
            _ => throw new NotImplementedException(),
        };
    }

    public bool IsFullIncomeDate(GetIncomeRequest request)
    {
        return request.IsPaymentType() || DateIsEqualToPaymentDate(request) || DateIsMoreThanPaymentDate(request);
    }

    private bool DateIsEqualToPaymentDate(GetIncomeRequest request)
    {
        return request.TillDate?.Date == Dates.MaturityDate.Date ||
               request.TillDate?.Date == Dates.OfferDate?.Date;
    }

    private bool DateIsMoreThanPaymentDate(GetIncomeRequest request)
    {
        return request.TillDate?.Date > Dates.MaturityDate.Date ||
               request.TillDate?.Date > Dates.OfferDate?.Date;
    }
}