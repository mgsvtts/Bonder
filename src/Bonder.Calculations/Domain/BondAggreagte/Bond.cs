﻿using Domain.BondAggreagte.Dto;
using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Identities;
using Domain.BondAggreagte.ValueObjects.Incomes;
using Shared.Domain.Common.Models;

namespace Domain.BondAggreagte;

public sealed class Bond : AggregateRoot<BondId>
{
    private List<Coupon> _coupons = [];
    private readonly List<Amortization> _amortizations = [];

    public string Name { get; private set; }
    public FullIncome Income { get; private set; }
    public Dates Dates { get; private set; }
    public int? Rating { get; private set; }
    public bool IsAmortized => _amortizations.Count != 0;
    public IReadOnlyList<Coupon> Coupons => _coupons.AsReadOnly();
    public IReadOnlyList<Amortization> Amortizations => _amortizations.AsReadOnly();

    public Bond(BondId id,
                string name,
                StaticIncome income,
                Dates dates,
                int? rating,
                IEnumerable<Coupon> coupons,
                IEnumerable<Amortization>? amortizations = null) : base(id)
    {
        _coupons = coupons.ToList();
        _amortizations = amortizations is not null ? amortizations.ToList() : _amortizations;

        Name = name;
        Dates = dates;
        Rating = rating;
        Income = new FullIncome(income, CouponIncome.None, AmortizationIncome.None);
        Income = Income with { CouponIncome = GetCouponIncome(new GetIncomeRequest(DateIntervalType.TillOfferDate)) };
    }

    public Bond UpdateRating(int? rating)
    {
        if (rating < 0)
        {
            throw new ArgumentException("Rating cannot be less than 0", nameof(rating));
        }

        Rating = rating;

        return this;
    }

    public Bond UpdateCoupons(IEnumerable<Coupon> coupons)
    {
        coupons ??= Array.Empty<Coupon>();

        _coupons = coupons.ToList();

        return this;
    }

    public FullIncome GetIncomeOnDate(GetIncomeRequest request)
    {
        var couponIncome = GetCouponIncome(request);
        var amortizationIncome = GetAmortizationIncome(request.DateFrom, request.DateTo);

        if (IsFullIncomeDate(request))
        {
            return new FullIncome(Income.StaticIncome, couponIncome, amortizationIncome);
        }

        return new FullIncome(StaticIncome.None, couponIncome, amortizationIncome);
    }

    private CouponIncome GetCouponIncome(GetIncomeRequest request)
    {
        var dateTo = GetTillDate(request);

        return CalculateCouponIncome(request.DateFrom, dateTo, request.ConsiderDividendCutOffDate);
    }

    private CouponIncome CalculateCouponIncome(DateOnly dateFrom, DateOnly dateTo, bool considerDividendCutOffDate)
    {
        var futureCoupons = Coupons.Where(x => x.CanGetCoupon(dateFrom, dateTo, considerDividendCutOffDate));

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

    private AmortizationIncome GetAmortizationIncome(DateOnly dateFrom, DateOnly dateTo)
    {
        if (Amortizations.Count == 0)
        {
            return AmortizationIncome.None;
        }

        var absoluteAmortization = Amortizations
        .Where(x => x.PaymentDate >= dateFrom && x.PaymentDate <= dateTo)
        .Sum(x => x.Payout);

        return new AmortizationIncome(absoluteAmortization, absoluteAmortization / Income.StaticIncome.AbsoluteNominal);
    }

    private CouponIncome GetOrdinaryCouponsIncome(IEnumerable<Coupon> futureCoupons)
    {
        var absoluteIncome = futureCoupons.Sum(x => x.Payout);

        return new CouponIncome(absoluteIncome, GetCouponPercentIncome(absoluteIncome));
    }

    private CouponIncome GetFloatingCouponsIncome(IEnumerable<Coupon> futureCoupons)
    {
        var latestCoupon = futureCoupons
        .Where(x => x.Payout != 0)
        .OrderByDescending(x => x.PaymentDate)
        .FirstOrDefault();

        if (latestCoupon == default)
        {
            latestCoupon = Coupons
            .OrderByDescending(x => x.PaymentDate)
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
            DateIntervalType.TillCustomDate => request.DateTo,
            _ => throw new NotImplementedException(),
        };
    }

    private bool IsFullIncomeDate(GetIncomeRequest request)
    {
        return (Dates.MaturityDate is not null || Dates.OfferDate is not null) &&
               (request.IsPaymentType() ||
                DateIsMoreOrEqualToPaymentDate(request));
    }

    private bool DateIsMoreOrEqualToPaymentDate(GetIncomeRequest request)
    {
        return request.DateTo >= Dates.MaturityDate ||
               request.DateTo >= Dates.OfferDate ||
               request.DateFrom >= Dates.MaturityDate ||
               request.DateFrom >= Dates.OfferDate;
    }

    public override string ToString()
    {
        return Name;
    }
}