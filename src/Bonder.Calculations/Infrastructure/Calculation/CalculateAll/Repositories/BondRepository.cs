using Domain.BondAggreagte;
using Domain.BondAggreagte.Abstractions;
using Domain.BondAggreagte.Abstractions.Dto;
using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Identities;
using Infrastructure.Calculation.Dto.BondRepository;
using Infrastructure.Common;
using Infrastructure.Common.Extensions;
using LinqToDB;
using LinqToDB.Data;
using Mapster;

namespace Infrastructure.Calculation.CalculateAll.Repositories;

public sealed class BondRepository : IBondRepository
{
    private readonly DbConnection _db;

    public BondRepository(DbConnection db)
    {
        _db = db;
    }

    public async Task AddAsync(IEnumerable<Bond> bonds, CancellationToken token = default)
    {
        var dbBonds = bonds.Adapt<List<Common.Models.Bond>>();

        var tasks = dbBonds.Select(x => Task.Run(() => SetBondValues(x))).ToList();

        await Task.WhenAll(tasks);

        await _db.BeginTransactionAsync(token);

        try
        {
            await _db.BulkCopyAsync(dbBonds, cancellationToken: token);
            await _db.BulkCopyAsync(dbBonds.SelectMany(x => x.Coupons), cancellationToken: token);
            await _db.BulkCopyAsync(dbBonds.SelectMany(x => x.Amortizations), cancellationToken: token);
        }
        catch
        {
            await _db.RollbackTransactionAsync(token);
            throw;
        }

        await _db.CommitTransactionAsync(token);
    }

    public async Task UpdateAsync(Bond bond, CancellationToken token = default)
    {
        var dbCoupons = bond.Coupons.Adapt<List<Common.Models.Coupon>>();
        var dbAmortizations = bond.Amortizations.Adapt<List<Common.Models.Amortization>>();
        SetValues(dbCoupons, dbAmortizations, bond.Identity);

        var dbBond = bond.Adapt<Common.Models.Bond>();
        dbBond.UpdatedAt = DateTime.Now;

        try
        {
            await _db.BeginTransactionAsync(token);

            await _db.Coupons
            .Where(x => x.BondId == bond.Identity.InstrumentId)
            .DeleteAsync(token);

            await _db.Amortizations
            .Where(x => x.BondId == bond.Identity.InstrumentId)
            .DeleteAsync(token);

            await _db.BulkCopyAsync(dbCoupons, cancellationToken: token);
            await _db.BulkCopyAsync(dbAmortizations, cancellationToken: token);

            await _db.UpdateAsync(dbBond, token: token);

            await _db.CommitTransactionAsync(token);
        }
        catch
        {
            await _db.RollbackTransactionAsync(token);
        }
    }

    public Task<int> CountAsync(CancellationToken token = default)
    {
        return _db.Bonds.CountAsync(token);
    }

    public async Task<List<Bond>> TakeRangeAsync(Range range, CancellationToken token = default)
    {
        var dbBonds = await _db.Bonds
        .LoadWith(x => x.Coupons)
        .Skip(range.Start.Value)
        .Take(range.End.Value - range.Start.Value)
        .ToListAsync(token: token);

        return dbBonds.Adapt<List<Bond>>();
    }

    public async Task<List<Bond>> GetAllFloatingAsync(CancellationToken token = default)
    {
        var dbBonds = await _db.Bonds
        .LoadWith(x => x.Coupons)
        .LoadWith(x => x.Amortizations)
        .Where(x => x.Coupons.Any(x => x.IsFloating))
        .ToListAsync(token: token);

        return dbBonds.Adapt<List<Bond>>();
    }

    public async Task<List<Bond>> GetByTickersAsync(IEnumerable<Ticker> tickers, CancellationToken token = default)
    {
        var dbBonds = await _db.Bonds
        .LoadWith(x => x.Coupons)
        .LoadWith(x => x.Amortizations)
        .Where(x => tickers.Select(x => x.Value).Contains(x.Ticker))
        .ToListAsync(token: token);

        return dbBonds.Adapt<List<Bond>>();
    }

    public async Task RefreshAsync(IEnumerable<Ticker> newBondTickers, CancellationToken token = default)
    {
        await _db.Bonds
        .Where(x => !newBondTickers.Select(x => x.Value).Contains(x.Ticker))
        .DeleteAsync(token: token);
    }

    public async Task<List<Ticker>> UpdateIncomesAsync(IEnumerable<KeyValuePair<Ticker, StaticIncome>> bonds, CancellationToken token = default)
    {
        var notFoundTickers = await _db.Bonds
        .Where(x => !bonds.Select(x => x.Key.ToString()).Contains(x.Ticker))
        .Select(x => x.Ticker)
        .ToListAsync(token: token);

        var updateParams = CreateUpdateParams(bonds, notFoundTickers);

        await _db.ExecuteAsync
        (
            """
                UPDATE public.bonds AS b
                SET price_percent = t.price_percent,
                    absolute_nominal = t.absolute_nominal,
                    absolute_price = t.absolute_price,
                    updated_at = CURRENT_TIMESTAMP
                FROM UNNEST(@ticker_array, @price_percent, @absolute_nominal, @absolute_price)
                AS t(ticker, price_percent, absolute_nominal, absolute_price)
                WHERE b.ticker = t.ticker;
            """,
            [
                new DataParameter("@ticker_array",  updateParams.Tickers),
                new DataParameter("@price_percent", updateParams.PricePercents),
                new DataParameter("@absolute_nominal",updateParams.AbsoluteNominals),
                new DataParameter("@absolute_price", updateParams.AbsolutePrices)
             ]
        );

        return notFoundTickers.Select(x => new Ticker(x)).ToList();
    }

    private static UpdatePricesParams CreateUpdateParams(IEnumerable<KeyValuePair<Ticker, StaticIncome>> bonds, List<string> notFoundTickers)
    {
        var updateParams = new UpdatePricesParams();
        foreach (var bond in bonds)
        {
            if (!notFoundTickers.Contains(bond.Key.ToString()))
            {
                updateParams.Add(bond);
            }
        }

        return updateParams;
    }

    public async Task<GetPriceSortedResponse> GetPriceSortedAsync(GetPriceSortedRequest filter,
                                                                  IEnumerable<Ticker>? tickers = null,
                                                                  IEnumerable<Guid>? uids = null,
                                                                  bool takeAll = false,
                                                                  CancellationToken token = default)
    {
        var query = _db.Bonds
        .WhereIf(tickers != null, x => tickers!.Select(x => x.Value).Contains(x.Ticker))
        .WhereIf(uids != null, x => uids!.Contains(x.Id))
        .Where(x => x.AbsolutePrice >= filter.PriceFrom)
        .Where(x => x.AbsolutePrice <= filter.PriceTo)
        .Where(x => x.Rating == null || x.Rating >= filter.RatingFrom && x.Rating <= filter.RatingTo)
        .Where(x => x.AbsoluteNominal >= filter.NominalFrom)
        .Where(x => x.AbsoluteNominal <= filter.NominalTo)
        .WhereIf(!filter.IncludeUnknownRatings, x => x.Rating != null);

        var bonds = await query
        .OrderByDescending(x => x.PricePercent)
        .SkipIf(!takeAll, (filter.PageInfo.CurrentPage - 1) * filter.PageInfo.ItemsOnPage)
        .TakeIf(!takeAll, filter.PageInfo.ItemsOnPage)
        .LoadWith(x => x.Coupons.Where(x => x.PaymentDate >= filter.DateFrom)
                                .Where(x => x.PaymentDate <= filter.DateTo))
        .LoadWith(x => x.Amortizations.Where(x => x.PaymentDate >= filter.DateFrom)
                                      .Where(x => x.PaymentDate <= filter.DateTo))
        .ToListAsync(token: token);

        if (takeAll)
        {
            return new GetPriceSortedResponse(null, bonds.Adapt<List<Bond>>());
        }

        var pageInfo = CreatePageInfo(filter, bonds, await query.CountAsync(token: token));

        return new GetPriceSortedResponse(pageInfo, bonds.Adapt<List<Bond>>());
    }

    private static PageInfo CreatePageInfo(GetPriceSortedRequest filter, List<Common.Models.Bond> bonds, int total)
    {
        var itemsOnPage = bonds.Count < filter.PageInfo.ItemsOnPage ? bonds.Count : filter.PageInfo.ItemsOnPage;
        var lastPage = total == itemsOnPage ? filter.PageInfo.CurrentPage : total / filter.PageInfo.ItemsOnPage + 1;

        var pageInfo = new PageInfo(filter.PageInfo.CurrentPage,
                                    lastPage,
                                    itemsOnPage,
                                    total);
        return pageInfo;
    }

    private static void SetBondValues(Common.Models.Bond bond)
    {
        bond.CreatedAt = DateTime.Now;
        bond.UpdatedAt = DateTime.Now;
        foreach (var coupon in bond.Coupons)
        {
            coupon.CreatedAt = DateTime.Now;
            coupon.BondId = bond.Id;
        }

        foreach (var amortization in bond.Amortizations)
        {
            amortization.CreatedAt = DateTime.Now;
            amortization.BondId = bond.Id;
        }
    }

    private static async void SetValues(List<Common.Models.Coupon> coupons, List<Common.Models.Amortization> amortizations, BondId id)
    {
        var tasks = coupons.Select(x => Task.Run(() =>
        {
            x.CreatedAt = DateTime.Now;
            x.BondId = id.InstrumentId;
        })).ToList();

        tasks.AddRange(amortizations.Select(x => Task.Run(() =>
        {
            x.CreatedAt = DateTime.Now;
            x.BondId = id.InstrumentId;
        })));

        await Task.WhenAll(tasks);
    }
}