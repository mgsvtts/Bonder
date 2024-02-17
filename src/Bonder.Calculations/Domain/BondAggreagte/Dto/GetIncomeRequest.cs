using Ardalis.GuardClauses;
using Domain.BondAggreagte.Dto.Guards;

namespace Domain.BondAggreagte.Dto;

public sealed class GetIncomeRequest
{
    public DateIntervalType Type { get; }
    public bool ConsiderDividendCutOffDate { get; }
    public decimal PriceFrom { get; }
    public decimal PriceTo { get; }
    public decimal NominalFrom { get; }
    public decimal NominalTo { get; }
    public decimal YearCouponFrom { get; }
    public decimal YearCouponTo { get; }
    public DateOnly DateFrom { get; }
    public DateOnly DateTo { get; }
    public int RatingFrom { get; }
    public int RatingTo { get; }
    public bool IncludeUnknownRatings { get; }

    public GetIncomeRequest(DateIntervalType type,
                            decimal priceFrom = 0,
                            decimal priceTo = decimal.MaxValue,
                            decimal nominalFrom = 0,
                            decimal nominalTo = decimal.MaxValue,
                            decimal yearCouponFrom = 0,
                            decimal yearCouponTo = decimal.MaxValue,
                            int ratingFrom = 0,
                            int ratingTo = 10,
                            DateOnly? dateFrom = null,
                            DateOnly? dateTo = null,
                            bool considerDividendCutOffDate = true,
                            bool includeUnknownRatings = true)
    {
        Guard.Against.OutOfRange(priceFrom, nameof(priceFrom), 0, priceTo, $"{nameof(priceFrom)} must be less than {nameof(priceTo)}");
        Guard.Against.OutOfRange(nominalFrom, nameof(nominalFrom), 0, nominalTo, $"{nameof(nominalFrom)} must be less than {nameof(nominalTo)}");
        Guard.Against.OutOfRange(yearCouponFrom, nameof(yearCouponFrom), 0, yearCouponTo, $"{nameof(yearCouponFrom)} must be less than {nameof(yearCouponTo)}");
        Guard.Against.OutOfRange(ratingFrom, nameof(ratingFrom), 0, ratingTo, $"{nameof(ratingFrom)} must be less than {nameof(ratingTo)}");

        (DateFrom, DateTo) = Guard.Against.CustomDateNotSetted(type, dateFrom, dateTo);

        PriceFrom = Guard.Against.Negative(priceFrom);
        PriceTo = Guard.Against.Negative(priceTo);
        NominalFrom = Guard.Against.Negative(nominalFrom);
        NominalTo = Guard.Against.Negative(nominalTo);
        YearCouponFrom = Guard.Against.Negative(yearCouponFrom);
        YearCouponTo = Guard.Against.Negative(yearCouponTo);
        RatingFrom = Guard.Against.Negative(ratingFrom);
        RatingTo = Guard.Against.Negative(ratingTo);
        Type = type;
        ConsiderDividendCutOffDate = considerDividendCutOffDate;
        IncludeUnknownRatings = includeUnknownRatings;
    }

    public bool IsPaymentType()
    {
        return Type is DateIntervalType.TillMaturityDate or DateIntervalType.TillOfferDate;
    }
}