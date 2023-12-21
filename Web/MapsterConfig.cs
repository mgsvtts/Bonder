using Application.Calculation.Common.CalculationService;
using Domain.BondAggreagte.ValueObjects;
using Mapster;
using MapsterMapper;
using Presentation.Controllers.BondController.CalculateJson;
using System.Reflection;
using Tinkoff.InvestApi.V1;

namespace Web.Extensions.Mapping;

public static class MapsterConfig
{
    public static void RegisterMapsterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig<Presentation.Controllers.BondController.CalculateJson.Coupon, Domain.BondAggreagte.ValueObjects.Coupon>
        .ForType()
        .MapWith(x => new Domain.BondAggreagte.ValueObjects.Coupon(x.Date, x.Payout));

        TypeAdapterConfig<Presentation.Controllers.BondController.CalculateJson.Bond, Domain.BondAggreagte.Bond>
        .ForType()
        .MapWith(x => Domain.BondAggreagte.Bond.Create(x.Ticker,
                                                       x.Name,
                                                       new Money(x.Price, x.Denomination),
                                                       x.EndDate,
                                                       x.Coupon.Adapt<Domain.BondAggreagte.ValueObjects.Coupon>()));

        TypeAdapterConfig<Domain.BondAggreagte.Bond, PriceBondResponse>
        .ForType()
        .MapWith(x => new PriceBondResponse(x.Id, x.Name, x.Money.Price));

        TypeAdapterConfig<Domain.BondAggreagte.Bond, CouponeIncomeBondResponse>
        .ForType()
        .MapWith(x => new CouponeIncomeBondResponse(x.Id, x.Name, x.GetCouponOnlyIncome()));

        TypeAdapterConfig<Domain.BondAggreagte.Bond, IncomeBondResponse>
        .ForType()
        .MapWith(x => new IncomeBondResponse(x.Id, x.Name, x.GetFullIncome()));

        TypeAdapterConfig<CalculatedBond, CalculatedBondResponse>
        .ForType()
        .MapWith(x => new CalculatedBondResponse(x.Bond.Id, x.Bond.Name, x.Priority));

        TypeAdapterConfig<(Tinkoff.InvestApi.V1.Bond Bond, GetBondCouponsResponse Coupon, decimal Price), Domain.BondAggreagte.Bond>
        .ForType()
        .MapWith(x => Domain.BondAggreagte.Bond.Create(x.Bond.Ticker, x.Bond.Name, new Money(x.Price, x.Bond.Nominal), x.Bond.MaturityDate.ToDateTime(), x.Coupon.Events.Adapt<IEnumerable<Domain.BondAggreagte.ValueObjects.Coupon>>()));

        TypeAdapterConfig<Tinkoff.InvestApi.V1.Coupon, Domain.BondAggreagte.ValueObjects.Coupon>
        .ForType()
        .MapWith(x => new Domain.BondAggreagte.ValueObjects.Coupon(x.CouponDate.ToDateTime(), x.PayOneBond));

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
        var mapperConfig = new Mapper(typeAdapterConfig);

        services.AddSingleton<IMapper>(mapperConfig);
    }
}