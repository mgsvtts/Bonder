using Application.Commands.AttachTinkoffToken;
using Domain.UserAggregate;
using Domain.UserAggregate.Entities;
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
        .MapWith(x => new Operation(x.Name,
                                    x.Description,
                                    x.Type,
                                    x.State,
                                    x.Date,
                                    x.Payout,
                                    x.Price,
                                    x.Commission,
                                    x.InstrumentType,
                                    x.InstrumentId,
                                    x.Quantity,
                                    x.RestQuantity,
                                    x.Trades.Adapt<IReadOnlyList<Domain.UserAggregate.ValueObjects.Trades.Trade>>()));

        TypeAdapterConfig<Operation, Infrastructure.Common.Models.Operation>
        .ForType()
        .MapWith(x => new Infrastructure.Common.Models.Operation
        {
            Date = x.Date,
            Payout = x.Payout,
            State = x.State,
            Type = x.Type,
            Commission = x.Commisison,
            Description = x.Description,
            Id = Guid.NewGuid(),
            InstrumentId = x.InstrumentId,
            InstrumentType = x.InstrumentType,
            Name = x.Name,
            Price = x.Price,
            Quantity = x.Quantity,
            RestQuantity = x.RestQuantity,
            Trades = x.Trades.Adapt<List<Infrastructure.Common.Models.Trade>>(),
        });

        TypeAdapterConfig<TinkoffOperation, Operation>
        .ForType()
        .MapWith(x => CustomMappings.FromTinkoffOperation(x));

        TypeAdapterConfig<Trade, Domain.UserAggregate.ValueObjects.Trades.Trade>
       .ForType()
       .MapWith(x => new Domain.UserAggregate.ValueObjects.Trades.Trade(x.Date, x.Quantity, x.Price.ToDecimal()));

        TypeAdapterConfig<Domain.UserAggregate.ValueObjects.Trades.Trade, Infrastructure.Common.Models.Trade>
      .ForType()
      .MapWith(x => new Infrastructure.Common.Models.Trade
      {
          Date = x.Date,
          Price = x.Price,
          Quantity = x.Quantity,
          Id = Guid.NewGuid()
      });

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
        return new Operation(operation.Name,
                             operation.Description,
                             MapOperationType(operation),
                             MapOperationState(operation),
                             operation.Date,
                             operation.Payment.ToDecimal(),
                             operation.ItemPrice.ToDecimal(),
                             operation.ItemCommission.ToDecimal(),
                             MapInstrumentType(operation),
                             string.IsNullOrEmpty(operation.InstrumentId) ? null : Guid.Parse(operation.InstrumentId),
                             operation.Quantity,
                             operation.RestQuantity,
                             operation.TradeInfo is not null ? operation.TradeInfo.Trades.Adapt<IReadOnlyList<Domain.UserAggregate.ValueObjects.Trades.Trade>>() : new List<Domain.UserAggregate.ValueObjects.Trades.Trade>().AsReadOnly());
    }

    private static PortfolioType MapPortfolioType(TinkoffAccount account)
    {
        if (account.Type == TinkoffAccountType.Ordinary)
        {
            return PortfolioType.Ordinary;
        }
        if (account.Type == TinkoffAccountType.IIS)
        {
            return PortfolioType.IIS;
        }

        return PortfolioType.Unknown;
    }

    private static OperationState MapOperationState(TinkoffOperation operation)
    {
        if (operation.Type == TinkoffOperationState.Executed)
        {
            return OperationState.Executed;
        }
        if (operation.Type == TinkoffOperationState.Canceled)
        {
            return OperationState.Canceled;
        }
        if (operation.Type == TinkoffOperationState.InProgress)
        {
            return OperationState.InProgress;
        }

        return OperationState.Unknown;
    }

    private static OperationType MapOperationType(TinkoffOperation operation)
    {
        if (operation.Type == TinkoffOperationType.Input)
        {
            return OperationType.Input;
        }
        if (operation.Type == TinkoffOperationType.Output)
        {
            return OperationType.Output;
        }
        if (operation.Type == TinkoffOperationType.Tax)
        {
            return OperationType.Tax;
        }
        if (operation.Type == TinkoffOperationType.CouponInput)
        {
            return OperationType.CouponInput;
        }

        return OperationType.Unknown;
    }

    public static InstrumentType MapInstrumentType(TinkoffOperation operation)
    {
        if (operation.InstrumentKind == TinkoffOperationKind.Bond)
        {
            return InstrumentType.Bond;
        }
        if (operation.InstrumentKind == TinkoffOperationKind.Share)
        {
            return InstrumentType.Share;
        }

        return InstrumentType.Unknown;
    }
}