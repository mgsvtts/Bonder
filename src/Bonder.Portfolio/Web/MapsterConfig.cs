using Application.Commands.Operations.Create;
using Application.Commands.Portfolios.ImportPortfolio;
using Application.Commands.Portfolios.ImportPortfolio.Dto;
using Application.Commands.Portfolios.RefreshPortfolio;
using Application.Queries.GetStats;
using Bonder.Calculation.Grpc;
using Domain.Common.Abstractions.Dto;
using Domain.UserAggregate;
using Domain.UserAggregate.Entities;
using Domain.UserAggregate.ValueObjects.Operations;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Domain.UserAggregate.ValueObjects.Users;
using Infrastructure.HttpClients.Dto.GetAccounts;
using Infrastructure.HttpClients.Dto.GetOperations;
using Infrastructure.HttpClients.Dto.GetPortfolios;
using Mapster;
using MapsterMapper;
using Presentation.Controllers;
using Presentation.Controllers.Operations.Dto.Create;
using Presentation.Controllers.Portfolios.Dto.GetStats;
using Presentation.Controllers.Portfolios.Dto.RefreshPortfolio;
using Shared.Domain.Common.ValueObjects;
using System.Reflection;

namespace Web;

public static class MapsterConfig
{
    public static void RegisterMapsterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig<(RefreshPortfolioRequest Request, Guid UserId), RefreshPortfolioCommand>
        .ForType()
        .MapWith(x => new RefreshPortfolioCommand(new UserId(x.UserId), new TinkoffToken(x.Request.TinkoffToken)));

        TypeAdapterConfig<(IEnumerable<ImportedOperation> Operations, IList<GrpcBond> Bonds), IEnumerable<Operation>>
       .ForType()
       .MapWith(x => x.Operations.Select(operation => CustomMappings.FromImportedOperation(operation, x.Bonds.FirstOrDefault(bond => operation.Ticker == bond.Ticker))));

        TypeAdapterConfig<(GetStatsRequest Request, Guid CurrentUserId), GetStatsQuery>
        .ForType()
        .MapWith(x => new GetStatsQuery(x.Request.Type, x.Request.Id, new UserId(x.CurrentUserId), x.Request.DateFrom, x.Request.DateTo));

        TypeAdapterConfig<Infrastructure.Common.Models.Portfolio, Portfolio>
        .ForType()
        .MapWith(x => new Portfolio(new PortfolioId(x.Id),
                                    new Totals(x.TotalBondPrice, x.TotalSharePrice, x.TotalEtfPrice, x.TotalCurrencyPrice, x.TotalFuturePrice, x.TotalPortfolioPrice),     
                                    new ValidatedString(x.Name),
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
        .MapWith(x => new User(new UserId(x.Id), x.Token != null ? new TinkoffToken(x.Token) : null, x.Portfolios.Adapt<IEnumerable<Portfolio>>()));

        TypeAdapterConfig<Infrastructure.Common.Models.Operation, Operation>
        .ForType()
        .MapWith(x => new Operation(new ValidatedString(x.Name),
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
                                    new ValidatedString(x.Description)));

        TypeAdapterConfig<Operation, Infrastructure.Common.Models.Operation>
        .ForType()
        .MapWith(x => new Infrastructure.Common.Models.Operation
        {
            Date = x.Date,
            Payout = x.Payout,
            State = x.State,
            Type = x.Type,
            Commission = x.Commisison,
            Description = x.Description != null ? x.Description.ToString() : null,
            Id = Guid.NewGuid(),
            InstrumentId = x.InstrumentId,
            InstrumentType = x.InstrumentType,
            Name = x.Name.ToString(),
            Price = x.Price,
            Quantity = x.Quantity,
            RestQuantity = x.RestQuantity,
            Trades = x.Trades.Adapt<List<Infrastructure.Common.Models.Trade>>(),
        });

        TypeAdapterConfig<CreateOperationRequest, CreateOperationCommand>
        .ForType()
        .MapWith(x => new CreateOperationCommand
        (
            new PortfolioId(x.PortfolioId),
            new ValidatedString(x.Name),
            x.Description != null ? new ValidatedString(x.Description) : null,
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
            x.Trades != null ? x.Trades.Select(x => new Domain.UserAggregate.ValueObjects.Trades.Trade(x.Date, x.Quantity, x.Price)) : null
        ));

        TypeAdapterConfig<CreateOperationCommand, Operation>
           .ForType()
           .MapWith(x => new Operation
           (
               x.Name,
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
               x.Trades,
               x.Description
           ));

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
            Token = x.Token != null ? x.Token.ToString() : null,
            Portfolios = x.Portfolios.Adapt<List<Infrastructure.Common.Models.Portfolio>>()
        });

        TypeAdapterConfig<ImportPortfolioCommand, AddPortfoliosToUserRequest>
        .ForType()
        .MapWith(x => new AddPortfoliosToUserRequest(x.BrokerType, x.Name, x.Streams));

        TypeAdapterConfig<Portfolio, Infrastructure.Common.Models.Portfolio>
        .ForType()
        .MapWith(x => new Infrastructure.Common.Models.Portfolio
        {
            Id = Guid.NewGuid(),
            Name = x.Name.ToString(),
            TotalBondPrice = x.Totals.TotalBondPrice,
            TotalPortfolioPrice = x.Totals.TotalPortfolioPrice,
            TotalSharePrice = x.Totals.TotalSharePrice,
            TotalCurrencyPrice = x.Totals.TotalCurrencyPrice,
            TotalEtfPrice = x.Totals.TotalEtfPrice,
            TotalFuturePrice = x.Totals.TotalFuturePrice,
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
        var type = MapPortfolioType(account);

        return new Portfolio(new PortfolioId(Guid.NewGuid()),
                             new Totals(portfolio.TotalBondPrice.ToDecimal(), 
                                        portfolio.TotalSharePrice.ToDecimal(),
                                        portfolio.TotalEtfPrice.ToDecimal(),
                                        portfolio.TotalCurrencyPrice.ToDecimal(),
                                        portfolio.TotalFuturePrice.ToDecimal(),
                                        portfolio.TotalPortfolioPrice.ToDecimal()),
                             new ValidatedString(account.Name),
                             type,
                             BrokerType.Tinkoff,
                             portfolio.Positions.Select(x => new Bond(Guid.Parse(x.InstrumentId), x.Quantity.ToDecimal())),
                             null,
                             new AccountId(account.Id));
    }

    public static Operation FromTinkoffOperation(TinkoffOperation operation)
    {
        return new Operation(new ValidatedString(operation.Name),
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
                             new ValidatedString(operation.Description));
    }

    public static Operation FromImportedOperation(ImportedOperation operation, GrpcBond? bond)
    {
        var type = MapOperationType(operation.Type);
        var date = operation.Date.ToDateTime(operation.Time);

        return new Operation(new ValidatedString(operation.Name),
                             type,
                             OperationState.Executed,
                             date,
                             (decimal)operation.Payout,
                             (decimal)operation.Price,
                             (decimal)operation.Commission,
                             InstrumentType.Bond,
                             (int)operation.Quantity,
                             0,
                             bond?.Id,
                             null,
                             new ValidatedString($"Импорт от {date:yyyy-MM-dd-HH-mm-ss}"));
    }

    public static Bond FromImportedBond(KeyValuePair<GrpcBond, int> bond)
    {
        return new Bond(bond.Key.Id, bond.Value);
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