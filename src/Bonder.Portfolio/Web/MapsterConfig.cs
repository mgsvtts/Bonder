using Application.AttachTinkoffToken;
using Domain.UserAggregate.ValueObjects;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Infrastructure.Dto.GetAccounts;
using Mapster;
using MapsterMapper;
using Presentation.Controllers.Dto.AttachToken;
using System.Reflection;

namespace Web;

public static class MapsterConfig
{
    public static void RegisterMapsterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig<AttachTokenRequest, AttachTinkoffTokenCommand>
        .ForType()
        .MapWith(x => new AttachTinkoffTokenCommand(new UserName(x.UserName), x.Token));

        TypeAdapterConfig<Infrastructure.Common.Models.User, Domain.UserAggregate.User>
       .ForType()
       .MapWith(x => new Domain.UserAggregate.User(new UserName(x.UserName), x.Token));

        TypeAdapterConfig<TinkoffAccount, Portfolio>
        .ForType()
        .MapWith(x => CustopMappings.FromTinkoffAccount(x));

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
        var mapperConfig = new Mapper(typeAdapterConfig);

        services.AddSingleton<IMapper>(mapperConfig);
    }
}

public static class CustopMappings
{
    public static Portfolio FromTinkoffAccount(TinkoffAccount account)
    {
        var status = account.Status == "ACCOUNT_STATUS_OPEN" ? PortfolioStatus.Open : PortfolioStatus.Closed;
        var type = account.Type switch
        {
            "ACCOUNT_TYPE_TINKOFF" => PortfolioType.Ordinary,
            "ACCOUNT_TYPE_TINKOFF_IIS" => PortfolioType.IIS,
            _ => PortfolioType.Unknown,
        };

        return new Portfolio(account.Id, account.Name, type, status);
    }
}