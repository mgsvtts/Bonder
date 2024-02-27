using Application.AttachTinkoffToken;
using Domain.UserAggregate;
using Domain.UserAggregate.ValueObjects;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Infrastructure.Dto.GetAccounts;
using Infrastructure.Dto.GetPortfolios;
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

        TypeAdapterConfig<Infrastructure.Common.Models.User, User>
        .ForType()
        .MapWith(x => new User(new UserName(x.UserName), x.Token, null));

        TypeAdapterConfig<Infrastructure.Common.Models.Portfolio, Portfolio>
        .ForType()
        .MapWith(x => new Portfolio(x.TotalBondPrice, x.Name, x.Type, x.Status, x.Bonds.Select(x => new Bond(x.BondId, x.Count))));

        TypeAdapterConfig<Infrastructure.Common.Models.User, User>
        .ForType()
        .MapWith(x => new User(new UserName(x.UserName), x.Token, x.Portfolios.Adapt<IEnumerable<Portfolio>>()));

        TypeAdapterConfig<User, Infrastructure.Common.Models.User>
        .ForType()
        .MapWith(x => new Infrastructure.Common.Models.User
        {
            UserName = x.Identity.Name,
            Token = x.TinkoffToken,
            Portfolios = x.Portfolios.Adapt<List<Infrastructure.Common.Models.Portfolio>>()
        });

        TypeAdapterConfig<Portfolio, Infrastructure.Common.Models.Portfolio>
        .ForType()
        .MapWith(x => new Infrastructure.Common.Models.Portfolio
        {
            Id = Guid.NewGuid(),
            Name = x.Name,
            TotalBondPrice = x.TotalBondPrice,
            Type = x.Type,
            Status = x.Status,
        });

        TypeAdapterConfig<(GetTinkoffPortfolioResponse Portfolio, TinkoffAccount Account), Portfolio>
        .ForType()
        .MapWith(x => CustopMappings.FromTinkoffAccount(x.Portfolio, x.Account));

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
        var mapperConfig = new Mapper(typeAdapterConfig);

        services.AddSingleton<IMapper>(mapperConfig);
    }
}

public static class CustopMappings
{
    public static Portfolio FromTinkoffAccount(GetTinkoffPortfolioResponse portfolio, TinkoffAccount account)
    {
        var totalBondPrice = portfolio.TotalBondPrice.ToDecimal();
        var status = account.Status == "ACCOUNT_STATUS_OPEN" ? PortfolioStatus.Open : PortfolioStatus.Closed;
        var type = account.Type switch
        {
            "ACCOUNT_TYPE_TINKOFF" => PortfolioType.Ordinary,
            "ACCOUNT_TYPE_TINKOFF_IIS" => PortfolioType.IIS,
            _ => PortfolioType.Unknown,
        };

        return new Portfolio(totalBondPrice, account.Name, type, status, portfolio.Positions.Select(x => new Bond(Guid.Parse(x.InstrumentId), x.Quantity.ToDecimal())));
    }
}