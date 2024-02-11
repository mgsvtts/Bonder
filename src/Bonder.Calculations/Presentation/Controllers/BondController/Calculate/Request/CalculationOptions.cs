using Domain.BondAggreagte.Dto;

namespace Presentation.Controllers.BondController.Calculate.Request;

public sealed record CalculationOptions(DateIntervalType? Type,
                                        bool ConsiderDividendCutOffDate,
                                        decimal? PriceFrom,
                                        decimal? PriceTo,
                                        decimal? NominalFrom,
                                        decimal? NominalTo,
                                        decimal? YearCouponFrom,
                                        decimal? YearCouponTo,
                                        DateOnly? DateFrom,
                                        DateOnly? DateTo,
                                        int? RatingFrom,
                                        int? RatingTo,
                                        bool? IncludeUnknownRatings);
