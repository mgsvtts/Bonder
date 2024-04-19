using Application.Commands.Analyze;
using Application.Commands.Analyze.Dto;
using Application.Commands.Calculation.CalculateTickers;
using Application.Queries.Common;
using Bonder.Calculation.Grpc;
using Domain.BondAggreagte;
using Domain.BondAggreagte.Abstractions.Dto.CalculateAll;
using Domain.BondAggreagte.Abstractions.Dto.GetPriceSorted;
using Domain.BondAggreagte.Abstractions.Dto.Moex;
using Domain.BondAggreagte.Dto;
using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Identities;
using Domain.BondAggreagte.ValueObjects.Incomes;
using Google.Protobuf.WellKnownTypes;
using Infrastructure.Calculation.Dto.GetAmortization;
using Infrastructure.Calculation.Dto.GetBonds.TInkoffApiData;
using Mapster;
using MapsterMapper;
using Presentation.Controllers.AnalyzeController.Analyze;
using Presentation.Controllers.BondController.Calculate.Request;
using Presentation.Controllers.BondController.Calculate.Response;
using Shared.Domain.Common;
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
            x.PageInfo != null ? new PageInfo(x.PageInfo.CurrentPage) : PageInfo.Default,
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

        TypeAdapterConfig<TinkoffValue, GetBondResponse>
        .ForType()
        .MapWith(x => CustomMappings.FromTinkoffValue(x));

        TypeAdapterConfig<AnalyzeBondsRequest, AdviceBondsCommand>
        .ForType()
        .MapWith(x => new AdviceBondsCommand(x.DefaultOptions, x.Bonds.Select(x => new Application.Commands.Analyze.BondToAnalyze(x.Option, new Ticker(x.Ticker)))));

        TypeAdapterConfig<Bond, BondItem>
        .ForType()
        .MapWith(x => new BondItem(x.Identity.InstrumentId,
                                   x.Identity.Ticker.ToString(),
                                   x.Identity.Isin.ToString(),
                                   x.Name,
                                   x.Income.StaticIncome.AbsolutePrice,
                                   x.Income.StaticIncome.AbsoluteNominal,
                                   x.Dates.MaturityDate,
                                   x.Dates.OfferDate,
                                   x.Rating));

        TypeAdapterConfig<IEnumerable<BondItem>, BondsResponse>
        .ForType()
        .MapWith(x => CustomMappings.FromBondItems(x));

        TypeAdapterConfig<CalculateAllResponse, GetCurrentBondsResponse>
        .ForType()
        .MapWith(x => CustomMappings.FromCalculateAll(x));

        TypeAdapterConfig<(Bond Bond, FullIncome Income), AdviceBondWithIncome>
        .ForType()
        .MapWith(x => new AdviceBondWithIncome(x.Bond.Identity.Ticker, x.Bond.Name, x.Bond.Income.StaticIncome.AbsolutePrice, x.Income.FullIncomePercent));

        TypeAdapterConfig<MoexItem, MoexResponse>
        .ForType()
        .MapWith(x => new MoexResponse(x.Coupons.Adapt<List<Coupon>>(),
                                       x.Amortizations.Adapt<List<Amortization>>()));

        TypeAdapterConfig<MoexAmortizationItem, Amortization>
        .ForType()
        .MapWith(x => new Amortization(x.Date, x.Payment ?? 0));

        TypeAdapterConfig<Dictionary<AdviceBondWithIncome, IEnumerable<AdviceBondWithIncome>>, IEnumerable<AdviceBondsResponse>>
        .ForType()
        .MapWith(x => CustomMappings.FromBondsWithIncome(x));

        TypeAdapterConfig<(GetBondResponse BondResponse, List<Coupon>? Coupons, int? Rating, MoexResponse MoexResponse), Bond>
        .ForType()
        .MapWith(x => CustomMappings.FromBondResponse(x.BondResponse, x.Coupons, x.Rating, x.MoexResponse));

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
            Amortizations = x.Amortizations.Adapt<List<Infrastructure.Common.Models.Amortization>>(),
            MaturityDate = x.Dates.MaturityDate,
            OfferDate = x.Dates.OfferDate,
            PricePercent = x.Income.StaticIncome.PricePercent,
            Rating = x.Rating,
            AbsoluteNominal = x.Income.StaticIncome.AbsoluteNominal,
            AbsolutePrice = x.Income.StaticIncome.AbsolutePrice
        });

        TypeAdapterConfig<CalculateAllResponse, CalculateResponse>
        .ForType()
        .MapWith(x => CustomMappings.FromCalculateAllResponse(x));

        TypeAdapterConfig<Infrastructure.Common.Models.Bond, Bond>
        .ForType()
        .MapWith(x => Bond.Create(new BondId(x.Id, new Ticker(x.Ticker), new Isin(x.Isin)),
                                                    x.Name,
                                                    StaticIncome.FromPercents(x.PricePercent, x.AbsolutePrice, x.AbsoluteNominal),
                                                    new Dates(x.MaturityDate, x.OfferDate),
                                                    x.Rating,
                                                    x.Coupons.Adapt<List<Coupon>>(),
                                                    x.Amortizations.Adapt<List<Amortization>>()));

        TypeAdapterConfig<Infrastructure.Common.Models.Coupon, Coupon>
       .ForType()
       .MapWith(x => new Coupon(x.PaymentDate, x.Payout, x.DividendCutOffDate, x.IsFloating));

        TypeAdapterConfig<Infrastructure.Common.Models.Amortization, Amortization>
       .ForType()
       .MapWith(x => new Amortization(x.PaymentDate, x.Payout));

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

        TypeAdapterConfig<Amortization, Infrastructure.Common.Models.Amortization>
       .ForType()
       .MapWith(x => new Infrastructure.Common.Models.Amortization
       {
           Id = Guid.NewGuid(),
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
    public static GetBondResponse FromTinkoffValue(TinkoffValue value)
    {
        var maturityDate = value.MaturityDate ?? value.CallDate;

        return new GetBondResponse(new BondId(value.Symbol.SecurityUids.InstrumentUid,
                                              new Ticker(value.Symbol.Ticker),
                                              new Isin(value.Symbol.Isin)),
                                   value.Symbol.Name,
                                   StaticIncome.FromAbsoluteValues(value.Price != null ? value.Price.Value : 0, value.Nominal),
                                   new Dates(maturityDate != null ? DateOnly.FromDateTime(maturityDate.Value) : null,
                                             value.OfferDate != null ? DateOnly.FromDateTime(value.OfferDate.Value) : null),
                                   value.IsAmortized);
    }

    public static BondsResponse FromBondItems(IEnumerable<BondItem> items)
    {
        var response = new BondsResponse();

        foreach (var item in items)
        {
            var offerDate = item.OfferDate is not null ? item.OfferDate.Value.ToDateTime(TimeOnly.MinValue) : DateTime.MinValue;
            var maturityDate = item.MaturityDate is not null ? item.MaturityDate.Value.ToDateTime(TimeOnly.MinValue) : DateTime.MinValue;

            response.Bonds.Add(new GrpcBond
            {
                Id = item.Id,
                Isin = item.Isin,
                Name = item.Name,
                Nominal = item.Nominal,
                Price = item.Price,
                Rating = item.Rating ?? 0,
                Ticker = item.Ticker,
                OfferDate = Timestamp.FromDateTime(offerDate.ToUniversalTime()),
                MaturityDate = Timestamp.FromDateTime(maturityDate.ToUniversalTime()),
            });
        }

        return response;
    }

    public static GetCurrentBondsResponse FromCalculateAll(CalculateAllResponse calculationResponse)
    {
        var response = new GetCurrentBondsResponse();

        foreach (var result in calculationResponse.Aggregation.FullIncomeSortedBonds)
        {
            response.Bonds.Add(new GetCurrentBondsItem
            {
                Id = result.Bond.Identity.InstrumentId,
                Name = result.Bond.Name,
                Price = result.Bond.Income.StaticIncome.AbsolutePrice,
                Ticker = result.Bond.Identity.Ticker.ToString(),
                Income = result.Money,
            });
        }

        return response;
    }

    public static Bond FromBondResponse(GetBondResponse bond, List<Coupon>? coupons, int? rating, MoexResponse moexResponse)
    {
        List<Coupon> couponsToAdd;
        List<Amortization>? amortizationsToAdd = null;
        if (coupons is not null)
        {
            couponsToAdd = coupons;
        }
        else
        {
            couponsToAdd = moexResponse.Coupons;
            amortizationsToAdd = moexResponse.Amortizations;
        }

        return Bond.Create(bond.BondId,
                           bond.Name,
                           bond.Income,
                           bond.Dates,
                           rating,
                           couponsToAdd,
                           amortizationsToAdd);
    }

    public static CalculateResponse FromCalculateAllResponse(CalculateAllResponse results)
    {
        return new CalculateResponse(CalculatedBonds: results.Aggregation.PrioritySortedBonds.Select(x => new CalculatedBondResponse(x.Bond.Identity.Ticker.Value, x.Bond.Name, x.Priority)),
                                     PriceSortedBonds: results.Aggregation.PriceSortedBonds.Select(x => new PriceBondResponse(x.Bond.Identity.Ticker.Value, x.Bond.Name, x.Money)),
                                     CouponIncomeSortedBonds: results.Aggregation.BondsWithIncome.Select(x => new IncomeBondResponse(x.Bond.Identity.Ticker.Value, x.Bond.Name, x.FullIncome.CouponIncome.CouponPercent)).OrderByDescending(x => x.Income),
                                     AmortizationIncomeSortedBonds: results.Aggregation.BondsWithIncome.Select(x => new IncomeBondResponse(x.Bond.Identity.Ticker.Value, x.Bond.Name, x.FullIncome.AmortizationIncome.AmortizationPercent)).OrderByDescending(x => x.Income),
                                     FullIncomeSortedBonds: results.Aggregation.FullIncomeSortedBonds.Select(x => new IncomeBondResponse(x.Bond.Identity.Ticker.Value, x.Bond.Name, x.Money)),
                                     CreditRatingSortedBonds: results.Aggregation.PriceSortedBonds.GroupBy(x => x.Bond.Rating)
                                                                                              .OrderBy(x => x.Key)
                                                                                              .Select(x => new CreditRatingBondResponse(x.Key, x.Select(x => new CreditRatingBond(x.Bond.Identity.Ticker.ToString(), x.Bond.Name)))),
                                     results.PageInfo);
    }

    public static IEnumerable<AdviceBondsResponse> FromBondsWithIncome(Dictionary<AdviceBondWithIncome, IEnumerable<AdviceBondWithIncome>> dict)
    {
        return dict.Select(a => new AdviceBondsResponse
        (
            a.Key.Id.ToString(),
            a.Key.Income,
            a.Value.Select(x => new AnalyzeBondsResponseBond(x.Id.ToString(),
                                                             x.Name,
                                                             x.Price,
                                                             x.Income))));
    }
}