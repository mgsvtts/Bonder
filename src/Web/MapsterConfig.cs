using Application.Calculation.CalculateTickers;
using Domain.BondAggreagte.Dto;
using Domain.BondAggreagte.ValueObjects;
using Infrastructure.Calculation.Dto.GetBonds.TInkoffApiData;
using Mapster;
using MapsterMapper;
using Presentation.Controllers.BondController.Calculate.Request;
using System.Reflection;

namespace Web.Extensions.Mapping;

public static class MapsterConfig
{
    public static void RegisterMapsterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig<CalculateRequest, CalculateTickersCommand>
        .ForType()
        .MapWith(x => new CalculateTickersCommand(new GetIncomeRequest(x.Options.Type, x.Options.TillDate, x.Options.ConsiderDividendCutOffDate),
                                                  x.Tickers.Select(x => new Ticker(x))));

        TypeAdapterConfig<(TinkoffValue Bond, List<Coupon> Coupons, int? Rating), Domain.BondAggreagte.Bond>
        .ForType()
        .MapWith(x => Domain.BondAggreagte.Bond.Create(new BondId(new Ticker(x.Bond.Symbol.Ticker),
                                                                  new Isin(x.Bond.Symbol.Isin)),
                                                       x.Bond.Symbol.Name,
                                                       new Money(x.Bond.Price != null ? x.Bond.Price.Value : 0, x.Bond.Nominal),
                                                       new Dates(x.Bond.MaturityDate, x.Bond.OfferDate),
                                                       x.Rating,
                                                       x.Coupons));

        TypeAdapterConfig<Tinkoff.InvestApi.V1.Coupon, Coupon>
        .ForType()
        .MapWith(x => new Coupon(x.CouponDate.ToDateTime(), x.PayOneBond, x.FixDate.ToDateTime(), x.CouponType == Tinkoff.InvestApi.V1.CouponType.Floating));

        TypeAdapterConfig<Domain.BondAggreagte.Bond, Infrastructure.Common.Models.Bond>
        .ForType()
        .MapWith(x => new Infrastructure.Common.Models.Bond
        {
            Name = x.Name,
            Ticker = x.Id.Ticker.Value,
            Isin = x.Id.Isin.Value,
            Coupons = x.Coupons.Adapt<List<Infrastructure.Common.Models.Coupon>>(),
            MaturityDate = x.Dates.MaturityDate,
            OfferDate = x.Dates.OfferDate,
            Price = x.Money.Price,
            NominalIncome = x.Money.NominalIncome,
            Rating = x.Rating
        });

        TypeAdapterConfig<Coupon, Infrastructure.Common.Models.Coupon>
        .ForType()
        .MapWith(x => new Infrastructure.Common.Models.Coupon
        {
            Id = Guid.NewGuid(),
            DividendCutOffDate = x.DividendCutOffDate,
            IsFloating = x.IsFloating,
            PaymentDate = x.PaymentDate,
            Payout = x.Payout
        });

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
        var mapperConfig = new Mapper(typeAdapterConfig);

        services.AddSingleton<IMapper>(mapperConfig);
    }
}