using Domain.BondAggreagte.Dto;
using Domain.BondAggreagte.Exceptions;
using Domain.BondAggreagte.ValueObjects;
using Domain.Common.Models;
using System.Xml.Linq;

namespace Domain.BondAggreagte;

public sealed class Bond : AggregateRoot<BondId>
{
    private readonly List<Coupon> _coupons = new List<Coupon>();

    public string Name { get; private set; }
    public FullIncome Income { get; private set; }
    public Dates Dates { get; private set; }
    public int? Rating { get; private set; }
    public bool IsAmortized { get; private set; }
    public IReadOnlyList<Coupon> Coupons => _coupons.AsReadOnly();

    public Bond(BondId id,
                string name,
                StaticIncome income,
                Dates dates,
                int? rating,
                bool isAmortized,
                IEnumerable<Coupon> coupons) : base(id)
    {
        Name = name;
        _coupons = coupons.ToList();
        Dates = dates;
        Rating = rating;
        IsAmortized = isAmortized;
        Income = new FullIncome(income, CouponIncome.None);
        Income = Income with { CouponIncome = GetCouponOnlyIncome(new GetIncomeRequest(DateIntervalType.TillOfferDate)) };
    }

    public FullIncome GetIncomeOnDate(GetIncomeRequest request)
    {
        var couponIncome = GetCouponOnlyIncome(request);

        if (IsFullIncomeDate(request))
        {
            return new FullIncome(Income.StaticIncome, couponIncome);
        }

        return new FullIncome(StaticIncome.None, couponIncome);
    }

    private CouponIncome GetCouponOnlyIncome(GetIncomeRequest request)
    {
        var date = GetTillDate(request);

        if (date < DateOnly.FromDateTime(DateTime.Now.Date))
        {
            throw new InvalidPaymentDateException(date);
        }

        return CalculateCouponIncome(date, request.ConsiderDividendCutOffDate);
    }

    private CouponIncome CalculateCouponIncome(DateOnly date, bool considerDividendCutOffDate)
    {
        var futureCoupons = Coupons.Where(x => x.CanGetCoupon(date, considerDividendCutOffDate));

        if (Coupons.Any(x => x.IsFloating))
        {
            return GetFloatingCouponsIncome(futureCoupons);
        }
        else if (Coupons.Count != 0)
        {
            return GetOrdinaryCouponsIncome(futureCoupons);
        }

        return CouponIncome.None;
    }

    private CouponIncome GetOrdinaryCouponsIncome(IEnumerable<Coupon> futureCoupons)
    {
        var absoluteIncome = futureCoupons.Sum(x => x.Payout);

        return new CouponIncome(absoluteIncome, GetCouponPercentIncome(absoluteIncome));
    }

    private CouponIncome GetFloatingCouponsIncome(IEnumerable<Coupon> futureCoupons)
    {
        var latestCoupon = futureCoupons.Where(x => x.Payout != 0)
                                        .OrderByDescending(x => x.PaymentDate)
                                        .FirstOrDefault();

        if (latestCoupon == default)
        {
            latestCoupon = Coupons.OrderByDescending(x => x.PaymentDate)
                                  .First();
        }

        var absoluteIncome = latestCoupon.Payout * futureCoupons.Count();

        return new CouponIncome(absoluteIncome, GetCouponPercentIncome(absoluteIncome));
    }

    private decimal GetCouponPercentIncome(decimal absoluteCouponIncome)
    {
        return absoluteCouponIncome / Income.StaticIncome.AbsoluteNominal;
    }

    private DateOnly GetTillDate(GetIncomeRequest request)
    {
        var maturityDate = Dates.MaturityDate != null ? Dates.MaturityDate : Coupons.OrderByDescending(x => x.PaymentDate).First().PaymentDate;
        var offerDate = Dates.OfferDate != null ? Dates.OfferDate : maturityDate;

        return request.Type switch
        {
            DateIntervalType.TillMaturityDate => maturityDate.Value,
            DateIntervalType.TillOfferDate => offerDate.Value,
            DateIntervalType.TillDate => request.TillDate.Value,
            _ => throw new NotImplementedException(),
        };
    }

    private bool IsFullIncomeDate(GetIncomeRequest request)
    {
        return Dates.MaturityDate is not null &&
               (request.IsPaymentType() ||
                DateIsEqualToPaymentDate(request) ||
                DateIsMoreThanPaymentDate(request));
    }

    private bool DateIsEqualToPaymentDate(GetIncomeRequest request)
    {
        return request.TillDate == Dates.MaturityDate ||
               request.TillDate == Dates.OfferDate;
    }

    private bool DateIsMoreThanPaymentDate(GetIncomeRequest request)
    {
        return request.TillDate > Dates.MaturityDate ||
               request.TillDate > Dates.OfferDate;
    }

    public override string ToString()
    {
        return Name;
    }
}
