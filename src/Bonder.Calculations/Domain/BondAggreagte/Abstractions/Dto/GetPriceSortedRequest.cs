using Domain.BondAggreagte.Dto;
using Domain.BondAggreagte.ValueObjects.Identities;

namespace Domain.BondAggreagte.Abstractions.Dto;

public sealed class GetPriceSortedRequest : GetIncomeRequest
{
    public PageInfo PageInfo { get; init; }

    public GetPriceSortedRequest(DateIntervalType type,
                                 PageInfo pageInfo,
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
                                 bool includeUnknownRatings = true) : base(type,
                                                                           priceFrom,
                                                                           priceTo,
                                                                           nominalFrom,
                                                                           nominalTo,
                                                                           yearCouponFrom,
                                                                           yearCouponTo,
                                                                           ratingFrom,
                                                                           ratingTo,
                                                                           dateFrom,
                                                                           dateTo,
                                                                           considerDividendCutOffDate,
                                                                           includeUnknownRatings)
    {
        PageInfo = pageInfo;
    }

    public bool IsDefault()
    {
        return (IntervalType == DateIntervalType.TillOfferDate || IntervalType == DateIntervalType.TillMaturityDate) &&
                PriceFrom == 0 &&
                PriceTo == decimal.MaxValue &&
                NominalFrom == 0 &&
                NominalTo == decimal.MaxValue &&
                YearCouponFrom == 0 &&
                YearCouponTo == decimal.MaxValue &&
                RatingFrom == 0 &&
                RatingTo == 10 &&
                IncludeUnknownRatings == true;
    }
}