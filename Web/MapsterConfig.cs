using Application.Calculation.Common.CalculationService;
using Domain.BondAggreagte.ValueObjects;
using Mapster;
using MapsterMapper;
using Presentation.Controllers.BondController.Calculate;
using System.Reflection;
using Tinkoff.InvestApi.V1;

namespace Web.Extensions.Mapping;

public static class MapsterConfig
{
    public static void RegisterMapsterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig<Domain.BondAggreagte.Bond, PriceBondResponse>
        .ForType()
        .MapWith(x => new PriceBondResponse(x.Id.Ticker.Value.ToString(), x.Id.Uid.ToString(), x.Id.Figi.Value.Value.ToString(), x.Name, x.Money.Price));

        TypeAdapterConfig<Domain.BondAggreagte.Bond, CouponeIncomeBondResponse>
        .ForType()
        .MapWith(x => new CouponeIncomeBondResponse(x.Id.Ticker.Value.ToString(), x.Id.Uid.ToString(), x.Id.Figi.Value.Value.ToString(), x.Name, x.GetCouponOnlyIncome()));

        TypeAdapterConfig<Domain.BondAggreagte.Bond, IncomeBondResponse>
        .ForType()
        .MapWith(x => new IncomeBondResponse(x.Id.Ticker.Value.ToString(), x.Id.Uid.ToString(), x.Id.Figi.Value.Value.ToString(), x.Name, x.GetFullIncome()));

        TypeAdapterConfig<CalculatedBond, CalculatedBondResponse>
        .ForType()
        .MapWith(x => new CalculatedBondResponse(x.Bond.Id.Ticker.Value.ToString(), x.Bond.Id.Uid.ToString(), x.Bond.Id.Figi.Value.Value.ToString(), x.Bond.Name, x.Priority));

        TypeAdapterConfig<(Tinkoff.InvestApi.V1.Bond Bond, GetBondCouponsResponse Coupon, decimal Price), Domain.BondAggreagte.Bond>
        .ForType()
        .MapWith(x => Domain.BondAggreagte.Bond.Create(new BondId(Guid.Parse(x.Bond.Uid), new Ticker(x.Bond.Ticker), !string.IsNullOrEmpty(x.Bond.Figi) ? new Figi(x.Bond.Figi) : null),
                                                       x.Bond.Name,
                                                       new Money(x.Price, x.Bond.Nominal),
                                                       x.Bond.MaturityDate.ToDateTime(),
                                                       x.Coupon.Events.Adapt<IEnumerable<Domain.BondAggreagte.ValueObjects.Coupon>>()));

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