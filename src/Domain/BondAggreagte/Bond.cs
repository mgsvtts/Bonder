using Domain.BondAggreagte.Dto;
using Domain.BondAggreagte.Exceptions;
using Domain.BondAggreagte.ValueObjects;
using Domain.Common.Models;
using System.Xml.Linq;

namespace Domain.BondAggreagte;

public class Bond : AggregateRoot<BondId>
{
    private readonly List<Coupon> _coupons = new List<Coupon>();

    public string Name { get; private set; }
    public FullIncome Income { get; private set; }
    public Dates Dates { get; private set; }
    public int? Rating { get; private set; }


    public IReadOnlyList<Coupon> Coupons => _coupons.AsReadOnly();

    public Bond(BondId id,
                 string name,
                 StaticIncome income,
                 Dates dates,
                 int? rating,
                 IEnumerable<Coupon> coupons) : base(id)
    {
        Name = name;
        _coupons = coupons.ToList();
        Dates = dates;
        Rating = rating;
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

        if (date < DateTime.Now)
        {
            throw new InvalidPaymentDateException(date);
        }

        return CalculateCouponIncome(date, request.ConsiderDividendCutOffDate);
    }

    private CouponIncome CalculateCouponIncome(DateTime date, bool considerDividendCutOffDate)
    {
        var futureCoupons = Coupons.Where(x => x.CanGetCoupon(date, considerDividendCutOffDate));

        if (Coupons.Any(x => x.IsFloating))
        {
            var latestCoupon = futureCoupons.Where(x => x.Payout != 0)
                                            .OrderByDescending(x => x.PaymentDate)
                                            .FirstOrDefault();

            latestCoupon ??= Coupons.OrderByDescending(x => x.PaymentDate)
                                    .First();

            var absoluteIncome = latestCoupon.Payout * futureCoupons.Count();
            var percentIncome = absoluteIncome / Income.StaticIncome.AbsoluteNominal;

            return new CouponIncome(absoluteIncome, percentIncome);
        }
        else if (Coupons.Count != 0)
        {
            var absoluteIncome = futureCoupons.Sum(x => x.Payout);
            var percentIncome = absoluteIncome / Income.StaticIncome.AbsoluteNominal;

            return new CouponIncome(absoluteIncome, percentIncome);
        }
        else
        {
            return CouponIncome.None;
        }
    }

    public override string ToString()
    {
        return Name;
    }

    private DateTime GetTillDate(GetIncomeRequest request)
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

    public bool IsFullIncomeDate(GetIncomeRequest request)
    {
        return Dates.MaturityDate is not null &&
               (request.IsPaymentType() ||
                DateIsEqualToPaymentDate(request) ||
                DateIsMoreThanPaymentDate(request));
    }

    private bool DateIsEqualToPaymentDate(GetIncomeRequest request)
    {
        return request.TillDate?.Date == Dates.MaturityDate?.Date ||
               request.TillDate?.Date == Dates.OfferDate?.Date;
    }

    private bool DateIsMoreThanPaymentDate(GetIncomeRequest request)
    {
        return request.TillDate?.Date > Dates.MaturityDate?.Date ||
               request.TillDate?.Date > Dates.OfferDate?.Date;
    }
}
