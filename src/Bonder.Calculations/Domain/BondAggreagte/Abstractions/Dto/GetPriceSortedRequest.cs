using Domain.BondAggreagte.Dto;

namespace Domain.BondAggreagte.Abstractions.Dto;

public class GetPriceSortedRequest : GetIncomeRequest
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
}