using Domain.UserAggregate.ValueObjects.Operations;
using Infrastructure.Common;
using LinqToDB;
using Mediator;
using Shared.Infrastructure.Extensions;

namespace Application.Queries.GetStats;
public sealed class GetStatsQueryHandler : IQueryHandler<GetStatsQuery, GetStatsResult>
{
    public async ValueTask<GetStatsResult> Handle(GetStatsQuery query, CancellationToken cancellationToken)
    {
        await ValidatePortfolio(query, cancellationToken);

        var fullPriceTask = GetFullPriceAsync(query, cancellationToken);
        var incomeTask = GetIncomeAsync(query, cancellationToken);
        var bondIncomeTask = GetBondIncomeAsync(query, cancellationToken);
        var shareIncomeTask = GetShareIncomeAsync(query, cancellationToken);
        var couponIncomeTask = GetCouponIncomeAsync(query, cancellationToken);
        var inputIncomeTask = GetInputIncomeAsync(query, cancellationToken);
        var sellIncomeTask = GetSellIncomeAsync(query, cancellationToken);
        var feeTask = GetFeeAsync(query, cancellationToken);
        var taxTask = GetTaxAsync(query, cancellationToken);
        var commissionTask = GetCommissionAsync(query, cancellationToken);
        var bondsPercentage = GetBondsPercentage(query, cancellationToken);

        await Task.WhenAll(fullPriceTask,
                           incomeTask,
                           bondIncomeTask,
                           shareIncomeTask,
                           couponIncomeTask,
                           inputIncomeTask,
                           sellIncomeTask,
                           feeTask,
                           taxTask,
                           commissionTask);

        return new GetStatsResult(new PercentItem(incomeTask.Result, fullPriceTask.Result),
                                  new PercentItem(feeTask.Result, fullPriceTask.Result),
                                  new PercentItem(commissionTask.Result, fullPriceTask.Result),
                                  new PercentItem(bondIncomeTask.Result, fullPriceTask.Result),
                                  new PercentItem(shareIncomeTask.Result, fullPriceTask.Result),
                                  new PercentItem(couponIncomeTask.Result, fullPriceTask.Result),
                                  new PercentItem(inputIncomeTask.Result, fullPriceTask.Result),
                                  new PercentItem(sellIncomeTask.Result, fullPriceTask.Result),
                                  new PercentItem(taxTask.Result, fullPriceTask.Result));
    }

    private static async Task<decimal> GetBondsPercentage(GetStatsQuery query, CancellationToken cancellationToken)
    {
        using var db = new DbConnection();

        if(query.Type == StatType.Portfolio)
        {
            var portfolioBonds = await db.Portfolios
                .Where(x => x.Id == query.Id)
                .SelectMany(x => x.Bonds)
                .ToListAsync(token: cancellationToken);

            return portfolioBonds.Count;
        }

        var userBonds = await db.Users
           .Where(x => x.Id == query.Id)
           .SelectMany(x => x.Portfolios.SelectMany(x=>x.Bonds))
           .ToListAsync(token: cancellationToken);

        return userBonds.Count;
    }

    private static async Task<decimal> GetCommissionAsync(GetStatsQuery query, CancellationToken cancellationToken)
    {
        using var db = new DbConnection();

        return await db.Operations
            .FilterOperations(query.Type, query.Id, query.DateFrom, query.DateTo)
            .SumAsync(x => x.Commission, token: cancellationToken);
    }

    private static async Task<decimal> GetTaxAsync(GetStatsQuery query, CancellationToken cancellationToken)
    {
        using var db = new DbConnection();

        return await db.Operations
                    .FilterOperations(query.Type, query.Id, query.DateFrom, query.DateTo)
                    .Where(x => x.Type == OperationType.Tax ||
                                x.Type == OperationType.BondTax)
                    .SumAsync(x => x.Payout, token: cancellationToken);
    }

    private static async Task<decimal> GetFeeAsync(GetStatsQuery query, CancellationToken cancellationToken)
    {
        using var db = new DbConnection();

        return await db.Operations
            .FilterOperations(query.Type, query.Id, query.DateFrom, query.DateTo)
            .Where(x => x.Type == OperationType.BrokerFee)
            .SumAsync(x => x.Payout, token: cancellationToken);
    }

    private static async Task<decimal> GetSellIncomeAsync(GetStatsQuery query, CancellationToken cancellationToken)
    {
        using var db = new DbConnection();

        return await db.Operations
            .FilterOperations(query.Type, query.Id, query.DateFrom, query.DateTo)
            .Where(x => x.Type == OperationType.Sell)
            .SumAsync(x => x.Payout, token: cancellationToken);
    }

    private static async Task<decimal> GetInputIncomeAsync(GetStatsQuery query, CancellationToken cancellationToken)
    {
        using var db = new DbConnection();

        return await db.Operations
            .FilterOperations(query.Type, query.Id, query.DateFrom, query.DateTo)
            .Where(x => x.Type == OperationType.Input)
            .SumAsync(x => x.Payout, token: cancellationToken);
    }

    private static async Task<decimal> GetCouponIncomeAsync(GetStatsQuery query, CancellationToken cancellationToken)
    {
        using var db = new DbConnection();

        return await db.Operations
                    .FilterOperations(query.Type, query.Id, query.DateFrom, query.DateTo)
                    .Where(x => x.Type == OperationType.CouponInput)
                    .SumAsync(x => x.Payout, token: cancellationToken);
    }

    private static async Task<decimal> GetShareIncomeAsync(GetStatsQuery query, CancellationToken cancellationToken)
    {
        using var db = new DbConnection();

        return await db.Operations
          .FilterOperations(query.Type, query.Id, query.DateFrom, query.DateTo)
          .Where(x => x.Type == OperationType.CouponInput ||
                      x.Type == OperationType.Input ||
                      x.Type == OperationType.Sell)
          .Where(x => x.InstrumentType == InstrumentType.Share)
          .SumAsync(x => x.Payout, token: cancellationToken);
    }

    private static async Task<decimal> GetBondIncomeAsync(GetStatsQuery query, CancellationToken cancellationToken)
    {
        using var db = new DbConnection();

        return await db.Operations
                   .FilterOperations(query.Type, query.Id, query.DateFrom, query.DateTo)
                   .Where(x => x.Type == OperationType.CouponInput ||
                               x.Type == OperationType.Input ||
                               x.Type == OperationType.Sell)
                   .Where(x => x.InstrumentType == InstrumentType.Bond)
                   .SumAsync(x => x.Payout, token: cancellationToken);
    }

    private static async Task<decimal> GetIncomeAsync(GetStatsQuery query, CancellationToken cancellationToken)
    {
        using var db = new DbConnection();

        return await db.Operations
                    .FilterOperations(query.Type, query.Id, query.DateFrom, query.DateTo)
                    .Where(x => x.Type == OperationType.CouponInput ||
                                x.Type == OperationType.Input ||
                                x.Type == OperationType.Sell)
                    .SumAsync(x => x.Payout, token: cancellationToken);
    }

    private static async Task<decimal> GetFullPriceAsync(GetStatsQuery query, CancellationToken cancellationToken)
    {
        using var db = new DbConnection();

        if (query.Type == StatType.Portfolio)
        {
            var item = await db.Portfolios.Select(x => new
            {
                x.Id,
                x.TotalPortfolioPrice
            }).FirstOrDefaultAsync(x => x.Id == query.Id, token: cancellationToken);

            return item.TotalPortfolioPrice;
        }

        var items = await db.Users
            .Where(x => x.Id == query.Id)
            .SelectMany(x => x.Portfolios)
            .ToListAsync(token: cancellationToken);

        return items.Sum(x => x.TotalPortfolioPrice);
    }

    private static async Task ValidatePortfolio(GetStatsQuery query, CancellationToken cancellationToken)
    {
        if (query.Type == StatType.User && query.CurrentUserId.Value != query.Id)
        {
            throw new UnauthorizedAccessException("You does not have access to this call");
        }

        if (query.Type != StatType.Portfolio)
        {
            return;
        }

        using var db = new DbConnection();

        var portfolio = await db.Portfolios
            .Where(x => x.Id == query.Id)
            .Where(x => x.UserId == query.CurrentUserId.Value)
            .FirstOrDefaultAsync(token: cancellationToken)
            ?? throw new ArgumentException("Portfolio does not exist");
    }
}

public static class OperationExtensions
{
    public static IQueryable<Infrastructure.Common.Models.Operation> FilterOperations(this IQueryable<Infrastructure.Common.Models.Operation> operations, StatType type, Guid id, DateTime dateFrom, DateTime dateTo)
    {
        return operations
            .WhereIf(type == StatType.Portfolio, x => x.PortfolioId == id)
            .WhereIf(type == StatType.User, x => x.Portfolio.UserId == id)
            .Where(x => x.State == OperationState.Executed)
            .Where(x => x.Date >= dateFrom)
            .Where(x => x.Date <= dateTo);
    }
}