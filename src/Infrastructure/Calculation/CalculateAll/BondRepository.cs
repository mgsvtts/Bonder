using System.Data;
using Domain.BondAggreagte;
using Domain.BondAggreagte.Repositories;
using Domain.BondAggreagte.ValueObjects;
using Infrastructure.Common;
using LinqToDB;
using LinqToDB.Data;
using MapsterMapper;

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

    public async Task UpdateRating(BondId id, int? rating, CancellationToken token = default)
    {
        await _db.Bonds.Where(x => x.Id == id.InstrumentId)
        .Set(x => x.Rating, rating)
        .Set(x => x.UpdatedAt, DateTime.Now)
        .UpdateAsync(token: token);
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

    public async Task UpdateCoupons(IEnumerable<Coupon> coupons, BondId id, CancellationToken token = default)
    {
        await _db.Coupons.Where(x => x.BondId == id.InstrumentId).DeleteAsync(token);

        var dbCoupons = _mapper.Map<List<Infrastructure.Common.Models.Coupon>>(coupons);

        SetCouponValues(dbCoupons, id);

        await _db.BulkCopyAsync(dbCoupons, cancellationToken: token);
    }

    public async Task<List<Bond>> GetAllFloatingAsync(CancellationToken token = default)
    {
        var dbBonds = await _db.Bonds
        .LoadWith(x => x.Coupons)
        .Where(x => x.Coupons.Any(x => x.IsFloating))
        .ToListAsync(token: token);

        return _mapper.Map<List<Bond>>(dbBonds);
    }

    public async Task RefreshAsync(IEnumerable<Ticker> newBondTickers, CancellationToken token = default)
    {
        await _db.Bonds.Where(x => !newBondTickers.Select(x => x.Value).Contains(x.Ticker))
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
            .Set(x => x.NominalPercent, value.NominalPercent)
            .Set(x => x.AbsoluteNominal, value.AbsoluteNominal)
            .Set(x => x.AbsolutePrice, value.AbsolutePrice)
            .Set(x => x.UpdatedAt, DateTime.Now)
            .UpdateAsync(token);
        }
        return notFoundTickers;
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
        var dbBonds = _mapper.Map<List<Infrastructure.Common.Models.Bond>>(bonds);

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

    private static void SetBondValues(Infrastructure.Common.Models.Bond bond)
    {
        bond.CreatedAt = DateTime.Now;
        foreach (var coupon in bond.Coupons)
        {
            coupon.BondId = bond.Id;
        }
    }

    private static async void SetCouponValues(List<Infrastructure.Common.Models.Coupon> coupons, BondId id)
    {
        var tasks = coupons.Select(x => Task.Run(() =>
        {
            x.CreatedAt = DateTime.Now;
            x.BondId = id.InstrumentId;
        })).ToList();

        await Task.WhenAll(tasks);
    }
}
