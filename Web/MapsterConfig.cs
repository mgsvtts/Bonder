using Mapster;
using MapsterMapper;
using System;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using Application.CalculateBonds;
using Domain.BondAggreagte.ValueObjects;
using Presentation.Controllers.BondController.CalculateJson;

namespace Web.Extensions.Mapping;

public static class MapsterConfig
{
    public static void RegisterMapsterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig<Presentation.Controllers.BondController.CalculateJson.Coupon, Domain.BondAggreagte.ValueObjects.Coupon>
        .ForType()
        .MapWith(x => new Domain.BondAggreagte.ValueObjects.Coupon(x.Date, x.Payout, x.PayRate));

        TypeAdapterConfig<Bond, Domain.BondAggreagte.Bond>
        .ForType()
        .MapWith(x => Domain.BondAggreagte.Bond.Create(x.Ticker,
                                                       x.Name,
                                                       x.Coupon.Adapt<Domain.BondAggreagte.ValueObjects.Coupon>(),
                                                       new Money(x.Price, x.Denomination),
                                                       x.EndDate));

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

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
        var mapperConfig = new Mapper(typeAdapterConfig);

        services.AddSingleton<IMapper>(mapperConfig);
    }
}