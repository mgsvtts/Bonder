using Domain.BondAggreagte;
using Domain.BondAggreagte.Abstractions;
using Domain.BondAggreagte.Abstractions.Dto;
using Domain.BondAggreagte.ValueObjects;
using Infrastructure.Common;
using Infrastructure.Common.Extensions;
using LinqToDB;
using LinqToDB.Data;
using MapsterMapper;
using System.Data;

namespace Infrastructure.Calculation.CalculateAll;

public sealed class BondRepository : IBondRepository
{
    private readonly DbConnection _db;

    private readonly IMapper _mapper;

    public BondRepository(DbConnection db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<int> CountAsync(CancellationToken token = default)
    {
        return await _db.Bonds.CountAsync(token);
    }

    public async Task UpdateAsync(Bond bond, CancellationToken token = default)
    {
        var dbCoupons = _mapper.Map<List<Common.Models.Coupon>>(bond.Coupons);
        SetCouponValues(dbCoupons, bond.Identity);

        var dbBond = _mapper.Map<Common.Models.Bond>(bond);
        dbBond.UpdatedAt = DateTime.Now;

        try
        {
            await _db.BeginTransactionAsync(token);

            await _db.Coupons
            .Where(x => x.BondId == bond.Identity.InstrumentId)
            .DeleteAsync(token);

            await _db.BulkCopyAsync(dbCoupons, cancellationToken: token);

            await _db.UpdateAsync(dbBond, token: token);

            await _db.CommitTransactionAsync(token);
        }
        catch
        {
            await _db.RollbackTransactionAsync(token);
        }
    }

    public async Task<List<Bond>> TakeRangeAsync(Range range, CancellationToken token = default)
    {
        var dbBonds = await _db.Bonds
        .LoadWith(x => x.Coupons)
        .Skip(range.Start.Value)
        .Take(range.End.Value - range.Start.Value)
        .ToListAsync(token: token);

        return _mapper.Map<List<Bond>>(dbBonds);
    }

    public async Task<List<Bond>> GetAllFloatingAsync(CancellationToken token = default)
    {
        var dbBonds = await _db.Bonds
        .LoadWith(x => x.Coupons)
        .Where(x => x.Coupons.Any(x => x.IsFloating))
        .ToListAsync(token: token);

        return _mapper.Map<List<Bond>>(dbBonds);
    }

    public async Task<List<Bond>> GetByTickersAsync(IEnumerable<Ticker> tickers, CancellationToken token = default)
    {
        var dbBonds = await _db.Bonds
        .LoadWith(x => x.Coupons)
        .Where(x => tickers.Select(x => x.Value).Contains(x.Ticker))
        .ToListAsync(token: token);

        return _mapper.Map<List<Bond>>(dbBonds);
    }

    public async Task RefreshAsync(IEnumerable<Ticker> newBondTickers, CancellationToken token = default)
    {
        await _db.Bonds
        .Where(x => !newBondTickers.Select(x => x.Value).Contains(x.Ticker))
        .DeleteAsync(token: token);
    }

    public async Task<List<Ticker>> UpdateIncomesAsync(IEnumerable<KeyValuePair<Ticker, StaticIncome>> bonds, CancellationToken token = default)
    {
        var notFoundTickers = new List<Ticker>();
        foreach (var (key, value) in bonds)
        {
            var dbBond = await _db.Bonds.FirstOrDefaultAsync(x => x.Ticker == key.ToString(), token: token);

            if (dbBond is null)
            {
                notFoundTickers.Add(key);
                continue;
            }

            await _db.Bonds
            .Where(x => x.Ticker == key.ToString())
            .Set(x => x.PricePercent, value.PricePercent)
            .Set(x => x.AbsoluteNominal, value.AbsoluteNominal)
            .Set(x => x.AbsolutePrice, value.AbsolutePrice)
            .Set(x => x.UpdatedAt, DateTime.Now)
            .UpdateAsync(token);
        }
        return notFoundTickers;
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
        .Where(x => x.MaturityDate >= filter.DateFrom || x.OfferDate >= filter.DateFrom)
        .Where(x => x.MaturityDate <= filter.DateTo || x.OfferDate <= filter.DateTo)
        .Where(x => x.AbsolutePrice >= filter.PriceFrom)
        .Where(x => x.AbsolutePrice <= filter.PriceTo)
        .Where(x => x.Rating == null || (x.Rating >= filter.RatingFrom && x.Rating <= filter.RatingTo))
        .Where(x => x.AbsoluteNominal >= filter.NominalFrom)
        .Where(x => x.AbsoluteNominal <= filter.NominalTo)
        .WhereIf(!filter.IncludeUnknownRatings, x => x.Rating != null);

        var bonds = await query
        .OrderByDescending(x => x.PricePercent)
        .SkipIf(!takeAll, (filter.PageInfo.CurrentPage - 1) * filter.PageInfo.ItemsOnPage)
        .TakeIf(!takeAll, filter.PageInfo.ItemsOnPage)
        .LoadWith(x => x.Coupons.Where(x => x.PaymentDate >= filter.DateFrom)
                                .Where(x => x.PaymentDate <= filter.DateTo))
        .ToListAsync(token: token);

        if (takeAll)
        {
            return new GetPriceSortedResponse(null, _mapper.Map<List<Bond>>(bonds));
        }

        var pageInfo = CreatePageInfo(filter, bonds, await query.CountAsync(token: token));

        return new GetPriceSortedResponse(pageInfo, _mapper.Map<List<Bond>>(bonds));
    }

    private static PageInfo CreatePageInfo(GetPriceSortedRequest filter, List<Common.Models.Bond> bonds, int total)
    {
        var itemsOnPage = bonds.Count < filter.PageInfo.ItemsOnPage ? bonds.Count : filter.PageInfo.ItemsOnPage;
        var lastPage = total == itemsOnPage ? filter.PageInfo.CurrentPage : (total / filter.PageInfo.ItemsOnPage) + 1;

        var pageInfo = new PageInfo(filter.PageInfo.CurrentPage,
                                    lastPage,
                                    itemsOnPage,
                                    total);
        return pageInfo;
    }

    public async Task<List<Bond>> GetPriceSortedAsync(CancellationToken token = default)
    {
        var bonds = await _db.Bonds
        .LoadWith(x => x.Coupons)
        .OrderBy(x => x.AbsolutePrice)
        .ToListAsync(token);

        return _mapper.Map<List<Bond>>(bonds);
    }

    public async Task AddAsync(IEnumerable<Bond> bonds, CancellationToken token = default)
    {
        var dbBonds = _mapper.Map<List<Common.Models.Bond>>(bonds);

        var tasks = dbBonds.Select(x => Task.Run(() => SetBondValues(x)));

        await Task.WhenAll(tasks);

        await _db.BeginTransactionAsync(token);

        try
        {
            await _db.BulkCopyAsync(dbBonds, cancellationToken: token);
            await _db.BulkCopyAsync(dbBonds.SelectMany(x => x.Coupons), cancellationToken: token);
        }
        catch
        {
            await _db.RollbackTransactionAsync(token);
            throw;
        }

        await _db.CommitTransactionAsync(token);
    }

    private static void SetBondValues(Common.Models.Bond bond)
    {
        bond.CreatedAt = DateTime.Now;
        foreach (var coupon in bond.Coupons)
        {
            coupon.BondId = bond.Id;
        }
    }

    private static async void SetCouponValues(List<Common.Models.Coupon> coupons, BondId id)
    {
        var tasks = coupons.Select(x => Task.Run(() =>
        {
            x.CreatedAt = DateTime.Now;
            x.BondId = id.InstrumentId;
        })).ToList();

        await Task.WhenAll(tasks);
    }
}