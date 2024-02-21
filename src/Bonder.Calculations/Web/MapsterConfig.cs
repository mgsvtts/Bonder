using Application.Calculation.CalculateAll.Services.Dto;
using Application.Calculation.CalculateTickers;
using Domain.BondAggreagte;
using Domain.BondAggreagte.Abstractions.Dto;
using Domain.BondAggreagte.Dto;
using Domain.BondAggreagte.ValueObjects;
using Infrastructure.Calculation.Dto.GetAmortization;
using Infrastructure.Calculation.Dto.GetBonds.TInkoffApiData;
using Mapster;
using MapsterMapper;
using Presentation.Controllers.BondController.Calculate.Request;
using Presentation.Controllers.BondController.Calculate.Response;
using System.Reflection;

namespace Web;

public static class MapsterConfig
{
    public static void RegisterMapsterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig<CalculateBondsRequest, CalculateBondsCommand>
        .ForType()
        .MapWith(x => new CalculateBondsCommand(x.Options.Adapt<GetPriceSortedRequest>(),
                                                x.IdType,
                                                x.Ids));

        TypeAdapterConfig<CalculationOptions, GetPriceSortedRequest>
        .ForType()
        .MapWith(x => new GetPriceSortedRequest
        (
            x.Type ?? DateIntervalType.TillOfferDate,
            x.PageInfo != null ? new PageInfo(x.PageInfo.CurrentPage, x.PageInfo.ItemsOnPage) : new PageInfo(1, 20),
            x.PriceFrom ?? 0,
            x.PriceTo ?? decimal.MaxValue,
            x.NominalFrom ?? 0,
            x.NominalTo ?? decimal.MaxValue,
            x.YearCouponFrom ?? 0,
            x.YearCouponTo ?? decimal.MaxValue,
            x.RatingFrom ?? 0,
            x.RatingTo ?? 10,
            x.DateFrom,
            x.DateTo,
            x.ConsiderDividendCutOffDate,
            x.IncludeUnknownRatings ?? true
        ));

        TypeAdapterConfig<TinkoffValue, Bond>
        .ForType()
        .MapWith(x => CustomMappings.CreateBond(x));

        TypeAdapterConfig<(Bond Bond, List<Coupon> Coupons, int? Rating), Bond>
        .ForType()
        .MapWith(x => new Bond(x.Bond.Identity,
                               x.Bond.Name,
                               x.Bond.Income.StaticIncome,
                               x.Bond.Dates,
                               x.Rating,
                               x.Bond.IsAmortized,
                               x.Coupons));

        TypeAdapterConfig<Tinkoff.InvestApi.V1.Coupon, Coupon>
        .ForType()
        .MapWith(x => new Coupon(DateOnly.FromDateTime(x.CouponDate.ToDateTime()),
                                 x.PayOneBond,
                                 DateOnly.FromDateTime(x.FixDate.ToDateTime()),
                                 x.CouponType == Tinkoff.InvestApi.V1.CouponType.Floating));

        TypeAdapterConfig<MoexCouponItem, Coupon>
        .ForType()
        .MapWith(x => new Coupon(x.Date, x.Payout ?? 0, x.CutOffDate, x.Payout == null));

        TypeAdapterConfig<Bond, Infrastructure.Common.Models.Bond>
        .ForType()
        .MapWith(x => new Infrastructure.Common.Models.Bond
        {
            Id = x.Identity.InstrumentId,
            Name = x.Name,
            Ticker = x.Identity.Ticker.Value,
            Isin = x.Identity.Isin.Value,
            Coupons = x.Coupons.Adapt<List<Infrastructure.Common.Models.Coupon>>(),
            MaturityDate = x.Dates.MaturityDate,
            OfferDate = x.Dates.OfferDate,
            PricePercent = x.Income.StaticIncome.PricePercent,
            Rating = x.Rating,
            AbsoluteNominal = x.Income.StaticIncome.AbsoluteNominal,
            AbsolutePrice = x.Income.StaticIncome.AbsolutePrice,
            IsAmortized = x.IsAmortized,
            CreatedAt = DateTime.Now
        });

        TypeAdapterConfig<CalculateAllResponse, CalculateResponse>
        .ForType()
        .MapWith(x => CustomMappings.MapToCalculateResponse(x));

        TypeAdapterConfig<Infrastructure.Common.Models.Bond, Bond>
        .ForType()
        .MapWith(x => new Bond(new BondId(x.Id, new Ticker(x.Ticker), new Isin(x.Isin)),
                                                    x.Name,
                                                    StaticIncome.FromPercents(x.PricePercent, x.AbsolutePrice, x.AbsoluteNominal),
                                                    new Dates(x.MaturityDate, x.OfferDate),
                                                    x.Rating,
                                                    x.IsAmortized,
                                                    x.Coupons.Adapt<List<Coupon>>()));

        TypeAdapterConfig<Infrastructure.Common.Models.Coupon, Coupon>
       .ForType()
       .MapWith(x => new Coupon(x.PaymentDate, x.Payout, x.DividendCutOffDate, x.IsFloating));

        TypeAdapterConfig<Coupon, Infrastructure.Common.Models.Coupon>
        .ForType()
        .MapWith(x => new Infrastructure.Common.Models.Coupon
        {
            Id = Guid.NewGuid(),
            DividendCutOffDate = x.DividendCutOffDate,
            IsFloating = x.IsFloating,
            PaymentDate = x.PaymentDate,
            CreatedAt = DateTime.Now,
            Payout = x.Payout
        });

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
        var mapperConfig = new Mapper(typeAdapterConfig);

        services.AddSingleton<IMapper>(mapperConfig);
    }
}

public static class CustomMappings
{
    public static Bond CreateBond(TinkoffValue value)
    {
        var maturityDate = value.MaturityDate ?? value.CallDate;

        return new Bond(new BondId(value.Symbol.SecurityUids.InstrumentUid,
                                   new Ticker(value.Symbol.Ticker),
                                   new Isin(value.Symbol.Isin)),
                        value.Symbol.Name,
                        StaticIncome.FromAbsoluteValues(value.Price != null ? value.Price.Value : 0, value.Nominal),
                        new Dates(maturityDate != null ? DateOnly.FromDateTime(maturityDate.Value) : null,
                                  value.OfferDate != null ? DateOnly.FromDateTime(value.OfferDate.Value) : null),
                        0,
                        value.IsAmortized,
                        new List<Coupon>());
    }

    public static CalculateResponse MapToCalculateResponse(CalculateAllResponse results)
    {
        return new CalculateResponse(CalculatedBonds: results.Results.Results.Select(x => new CalculatedBondResponse(x.Bond.Identity.Ticker.Value, x.Bond.Name, x.Priority)),
                                     PriceSortedBonds: results.Results.PriceSortedBonds.Select(x => new PriceBondResponse(x.Bond.Identity.Ticker.Value, x.Bond.Name, x.Money)),
                                     CouponIncomeSortedBonds: results.Results.Bonds.Select(x => new IncomeBondResponse(x.Key.Identity.Ticker.Value, x.Key.Name, x.Value.CouponIncome.CouponPercent)).OrderByDescending(x => x.Income),
                                     FullIncomeSortedBonds: results.Results.FullIncomeSortedBonds.Select(x => new IncomeBondResponse(x.Bond.Identity.Ticker.Value, x.Bond.Name, x.Money)),
                                     CreditRatingSortedBonds: results.Results.PriceSortedBonds.GroupBy(x => x.Bond.Rating)
                                                                                              .OrderBy(x => x.Key)
                                                                                              .Select(x => new CreditRatingBondResponse(x.Key, x.Select(x => new CreditRatingBond(x.Bond.Identity.Ticker.ToString(), x.Bond.Name)))),
                                     results.PageInfo);
    }
}