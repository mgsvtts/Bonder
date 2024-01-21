using Domain.BondAggreagte.Dto;
using Domain.BondAggreagte.Exceptions;
using Domain.BondAggreagte.ValueObjects;
using Domain.Common.Models;

namespace Domain.BondAggreagte;

public class Bond : AggregateRoot<BondId>
{
    private List<Coupon> _coupons = new List<Coupon>();

    public string Name { get; private set; }
    public IncomePercents Percents { get; private set; }
    public OriginalMoney Money { get; private set; }
    public Dates Dates { get; private set; }
    public int? Rating { get; private set; }
    public IReadOnlyList<Coupon> Coupons => _coupons.AsReadOnly();

    private Bond(BondId id) : base(id)
    { }

    public static Bond Create(BondId id,
                              string name,
                              IncomePercents percents,
                              OriginalMoney money,
                              Dates dates,
                              int? rating,
                              IEnumerable<Coupon> coupons)
    {
        return new Bond(id)
        {
            Name = name,
            _coupons = coupons.ToList(),
            Percents = percents,
            Dates = dates,
            Rating = rating,
            Money = money
        };
    }

    public Income GetIncome(GetIncomeRequest request)
    {
        var couponIncome = GetCouponOnlyIncomePercent(request);

        if (IsFullIncomeDate(request))
        {
            return new Income(couponIncome, Percents.NominalPercent);
        }

        return new Income(couponIncome);
    }

    private decimal GetCouponOnlyIncomePercent(GetIncomeRequest request)
    {
        var date = GetTillDate(request);

        if (date < DateTime.Now)
        {
            throw new InvalidPaymentDateException(date);
        }

        return CalculateCouponIncomePercent(date, request.ConsiderDividendCutOffDate);
    }

    private decimal CalculateCouponIncomePercent(DateTime date, bool considerDividendCutOffDate)
    {
        var futureCoupons = Coupons.Where(x => x.CanGetCoupon(date, considerDividendCutOffDate));

        if (Coupons.Any(x => x.IsFloating))
        {
            var latestCoupon = futureCoupons.Where(x => x.Payout != 0)
                                            .OrderByDescending(x => x.PaymentDate)
                                            .FirstOrDefault();

            latestCoupon ??= Coupons.OrderByDescending(x => x.PaymentDate)
                                    .First();

            return (latestCoupon.Payout * futureCoupons.Count()) / Money.OriginalNominal;
        }
        else if (Coupons.Count != 0)
        {
            return futureCoupons.Sum(x => x.Payout) / Money.OriginalNominal;
        }
        else
        {
            return 0;
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
