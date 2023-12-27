using Application.Calculation.CalculateTickers;
using Application.Calculation.Common.CalculationService;
using Domain.BondAggreagte.Dto;
using Domain.BondAggreagte.ValueObjects;
using Infrastructure.Calculation.Dto.GetBonds.TInkoffApiData;
using Mapster;
using MapsterMapper;
using Presentation.Controllers.BondController.Calculate.Request;
using Presentation.Controllers.BondController.Calculate.Response;
using System.Reflection;

namespace Web.Extensions.Mapping;

public static class MapsterConfig
{
    public static void RegisterMapsterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig<CalculateRequest, CalculateTickersCommand>
        .ForType()
        .MapWith(x => new CalculateTickersCommand(new GetIncomeRequest(x.Options.Type, x.Options.TillDate), x.Tickers.Select(x => new Ticker(x))));

        TypeAdapterConfig<(TinkoffValue Bond, List<Domain.BondAggreagte.ValueObjects.Coupon> Coupons), Domain.BondAggreagte.Bond>
        .ForType()
        .MapWith(x => Domain.BondAggreagte.Bond.Create(new Ticker(x.Bond.Symbol.Ticker), x.Bond.Symbol.Name, new Money(x.Bond.Price.Value, x.Bond.Nominal), new Dates(x.Bond.MaturityDate, x.Bond.OfferDate), x.Coupons));

        TypeAdapterConfig<Tinkoff.InvestApi.V1.Coupon, Domain.BondAggreagte.ValueObjects.Coupon>
        .ForType()
        .MapWith(x => new Domain.BondAggreagte.ValueObjects.Coupon(x.CouponDate.ToDateTime(), x.PayOneBond));

        TypeAdapterConfig<CalculationResult, CalculatedBondResponse>
        .ForType()
        .MapWith(x => new CalculatedBondResponse(x.Bond.Id.Value, x.Bond.Name, x.Priority));

        TypeAdapterConfig<CalculationResult, PriceBondResponse>
        .ForType()
        .MapWith(x => new PriceBondResponse(x.Bond.Id.Value, x.Bond.Name, x.Price));

        TypeAdapterConfig<CalculationResult, CouponeIncomeBondResponse>
        .ForType()
        .MapWith(x => new CouponeIncomeBondResponse(x.Bond.Id.Value, x.Bond.Name, x.CouponIncome));

        TypeAdapterConfig<CalculationResult, IncomeBondResponse>
        .ForType()
        .MapWith(x => new IncomeBondResponse(x.Bond.Id.Value, x.Bond.Name, x.Income));

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
        var mapperConfig = new Mapper(typeAdapterConfig);

        services.AddSingleton<IMapper>(mapperConfig);
    }
}