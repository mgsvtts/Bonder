using Application.Commands.ImportPortfolio.Dto;
using Application.Commands.RefreshPortfolio;
using Bonder.Calculation.Grpc;
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
using System.Reflection;

namespace Web;

public static class MapsterConfig
{
    public static void RegisterMapsterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig<(string TinkoffToken, Guid UserId), RefreshPortfolioCommand>
        .ForType()
        .MapWith(x => new RefreshPortfolioCommand(new UserId(x.UserId), new TinkoffToken(x.TinkoffToken)));

        TypeAdapterConfig<(IEnumerable<ImportedOperation> Operations, IList<GrpcBond> Bonds), IEnumerable<Operation>>
       .ForType()
       .MapWith(x => x.Operations.Select(operation => CustomMappings.FromImportedOperation(operation, x.Bonds.FirstOrDefault(bond => operation.Ticker == bond.Ticker))));

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

        TypeAdapterConfig<Dictionary<GrpcBond, int>, IEnumerable<Bond>>
       .ForType()
       .MapWith(x => x.Select(x => CustomMappings.FromImportedBond(x)));

        TypeAdapterConfig<Infrastructure.Common.Models.User, User>
        .ForType()
        .MapWith(x => new User(new UserId(x.Id), new TinkoffToken(x.Token), x.Portfolios.Adapt<IEnumerable<Portfolio>>()));

        TypeAdapterConfig<Infrastructure.Common.Models.Operation, Operation>
        .ForType()
        .MapWith(x => new Operation(x.Name,
                                    x.Type,
                                    x.State,
                                    x.Date,
                                    x.Payout,
                                    x.Price,
                                    x.Commission,
                                    x.InstrumentType,
                                    x.Quantity,
                                    x.RestQuantity,
                                    x.InstrumentId,
                                    x.Trades.Adapt<IEnumerable<Domain.UserAggregate.ValueObjects.Trades.Trade>>(),
                                    x.Description));

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
                             MapOperationType(operation),
                             MapOperationState(operation),
                             operation.Date,
                             operation.Payment.ToDecimal(),
                             operation.ItemPrice.ToDecimal(),
                             operation.ItemCommission.ToDecimal(),
                             MapInstrumentType(operation),
                             operation.Quantity,
                             operation.RestQuantity,
                             string.IsNullOrEmpty(operation.InstrumentId) ? null : Guid.Parse(operation.InstrumentId),
                             operation.TradeInfo is not null ? operation.TradeInfo.Trades.Adapt<IEnumerable<Domain.UserAggregate.ValueObjects.Trades.Trade>>() : Enumerable.Empty<Domain.UserAggregate.ValueObjects.Trades.Trade>(),
                             operation.Description);
    }

    public static Operation FromImportedOperation(ImportedOperation operation, GrpcBond? bond)
    {
        var type = MapOperationType(operation.Type);
        var date = operation.Date.ToDateTime(operation.Time);

        return new Operation(operation.Name,
                             type,
                             OperationState.Executed,
                             date,
                             (decimal)operation.Payout,
                             (decimal)operation.Price,
                             (decimal)operation.Commission,
                             InstrumentType.Bond,
                             (int)operation.Quantity,
                             0,
                             bond is not null ? Guid.Parse(bond.Id) : null,
                             null,
                             $"Импорт от {date:yyyy-MM-dd-HH-mm-ss}");
    }

    public static Bond FromImportedBond(KeyValuePair<GrpcBond, int> bond)
    {
        return new Bond(Guid.Parse(bond.Key.Id), bond.Value);
    }

    private static OperationType MapOperationType(string type)
    {
        return type switch
        {
            ImportedOperationType.Buy => OperationType.Buy,
            ImportedOperationType.Sell => OperationType.Sell,
            _ => OperationType.Unknown
        };
    }

    private static PortfolioType MapPortfolioType(TinkoffAccount account)
    {
        return account.Type switch
        {
            TinkoffAccountType.Ordinary => PortfolioType.Ordinary,
            TinkoffAccountType.IIS => PortfolioType.IIS,
            _ => PortfolioType.Unknown
        };
    }

    private static OperationState MapOperationState(TinkoffOperation operation)
    {
        return operation.State switch
        {
            TinkoffOperationState.Executed => OperationState.Executed,
            TinkoffOperationState.Canceled => OperationState.Canceled,
            TinkoffOperationState.InProgress => OperationState.InProgress,
            _ => OperationState.Unknown
        };
    }

    private static OperationType MapOperationType(TinkoffOperation operation)
    {
        return operation.Type switch
        {
            TinkoffOperationType.CouponInput => OperationType.CouponInput,
            TinkoffOperationType.Input => OperationType.Input,
            TinkoffOperationType.Output => OperationType.Output,
            TinkoffOperationType.BondTax => OperationType.BondTax,
            TinkoffOperationType.Tax => OperationType.Tax,
            TinkoffOperationType.Sell => OperationType.Sell,
            TinkoffOperationType.BrokerFee => OperationType.BrokerFee,
            TinkoffOperationType.Buy => OperationType.Buy,
            TinkoffOperationType.MoneyTransfer => OperationType.MoneyTransfer,
            TinkoffOperationType.AssetsTransfer => OperationType.AssetsTransfer,
            _ => OperationType.Unknown
        };
    }

    public static InstrumentType MapInstrumentType(TinkoffOperation operation)
    {
        return operation.InstrumentKind switch
        {
            TinkoffOperationKind.Bond => InstrumentType.Bond,
            TinkoffOperationKind.Share => InstrumentType.Share,
            _ => InstrumentType.Unknown
        };
    }
}