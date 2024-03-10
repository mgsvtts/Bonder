using Application.Commands.AttachTinkoffToken;
using Domain.UserAggregate;
using Domain.UserAggregate.Entities;
using Domain.UserAggregate.ValueObjects;
using Domain.UserAggregate.ValueObjects.Operations;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Domain.UserAggregate.ValueObjects.Users;
using Infrastructure.Dto.GetAccounts;
using Infrastructure.Dto.GetOperations;
using Infrastructure.Dto.GetPortfolios;
using Mapster;
using MapsterMapper;
using Presentation.Controllers.Dto.AttachToken;
using System.Reflection;
using System.Security.Principal;

namespace Web;

public static class MapsterConfig
{
    public static void RegisterMapsterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig<AttachTokenRequest, AttachTinkoffTokenCommand>
        .ForType()
        .MapWith(x => new AttachTinkoffTokenCommand(new UserId(x.UserId), new TinkoffToken(x.Token)));

        TypeAdapterConfig<Infrastructure.Common.Models.Portfolio, Portfolio>
        .ForType()
        .MapWith(x => new Portfolio(new PortfolioId(x.Id),
                                    x.TotalBondPrice,
                                    x.Name,
                                    x.Type,
                                    x.BrokerType,
                                    x.Bonds.Select(x => new Bond(x.BondId, x.Count)),
                                    x.Operations.Adapt<IEnumerable<Operation>>(),
                                    x.AccountId != null ? new AccountId(x.AccountId) : null));

        TypeAdapterConfig<Infrastructure.Common.Models.User, User>
        .ForType()
        .MapWith(x => new User(new UserId(x.Id), new TinkoffToken(x.Token), x.Portfolios.Adapt<IEnumerable<Portfolio>>()));

        TypeAdapterConfig<Infrastructure.Common.Models.Operation, Operation>
        .ForType()
        .MapWith(x => new Operation(x.Type, x.State, x.Date, x.Payout));

        TypeAdapterConfig<Operation, Infrastructure.Common.Models.Operation>
        .ForType()
        .MapWith(x => new Infrastructure.Common.Models.Operation
        {
            Date = x.Date,
            Payout = x.Payout,
            State = x.State,
            Type = x.Type
        });

        TypeAdapterConfig<TinkoffOperation, Operation>
        .ForType()
        .MapWith(x => CustomMappings.FromTinkoffOperation(x));

        TypeAdapterConfig<User, Infrastructure.Common.Models.User>
        .ForType()
        .MapWith(x => new Infrastructure.Common.Models.User
        {
            Id = x.Identity.Value,
            Token = x.Token.ToString(),
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
            BrokerType = x.BrokerType,
            AccountId = x.AccountId != null ? x.AccountId.ToString() : null,
            Operations = x.Operations.Adapt<List<Infrastructure.Common.Models.Operation>>()
        });

        TypeAdapterConfig<(GetTinkoffPortfolioResponse Portfolio, TinkoffAccount Account), Portfolio>
        .ForType()
        .MapWith(x => CustomMappings.FromTinkoffAccount(x.Portfolio, x.Account));

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
        var mapperConfig = new Mapper(typeAdapterConfig);

        services.AddSingleton<IMapper>(mapperConfig);
    }
}

public static class CustomMappings
{
    public static Portfolio FromTinkoffAccount(GetTinkoffPortfolioResponse portfolio, TinkoffAccount account)
    {
        var totalBondPrice = portfolio.TotalBondPrice.ToDecimal();

        var type = MapPortfolioType(account);

        return new Portfolio(new PortfolioId(Guid.NewGuid()),
                             totalBondPrice,
                             account.Name,
                             type,
                             BrokerType.Tinkoff,
                             portfolio.Positions.Select(x => new Bond(Guid.Parse(x.InstrumentId), x.Quantity.ToDecimal())),
                             null,
                             new AccountId(account.Id));
    }

    public static Operation FromTinkoffOperation(TinkoffOperation operation)
    {
        return new Operation(MapOperationType(operation), MapOperationState(operation), operation.Date, operation.Payment.ToDecimal());
    }

    private static PortfolioType MapPortfolioType(TinkoffAccount account)
    {
        PortfolioType type;
        if (account.Type == TinkoffAccountType.Ordinary)
        {
            type = PortfolioType.Ordinary;
        }
        else if (account.Type == TinkoffAccountType.IIS)
        {
            type = PortfolioType.IIS;
        }
        else
        {
            type = PortfolioType.Unknown;
        }

        return type;
    }

    private static OperationState MapOperationState(TinkoffOperation operation)
    {
        if (operation.Type == TinkoffOperationState.Executed)
        {
            return OperationState.Executed;
        }
        else if (operation.Type == TinkoffOperationState.Canceled)
        {
            return OperationState.Canceled;
        }
        else if (operation.Type == TinkoffOperationState.InProgress)
        {
            return OperationState.InProgress;
        }
        else
        {
            return OperationState.Unknown;
        }
    }

    private static OperationType MapOperationType(TinkoffOperation operation)
    {
        if (operation.Type == TinkoffOperationType.Input)
        {
            return OperationType.Input;
        }
        else if (operation.Type == TinkoffOperationType.Output)
        {
            return OperationType.Output;
        }
        else if (operation.Type == TinkoffOperationType.Tax)
        {
            return OperationType.Tax;
        }
        else if (operation.Type == TinkoffOperationType.CouponInput)
        {
            return OperationType.CouponInput;
        }
        else
        {
            return OperationType.Unknown;
        }
    }
}