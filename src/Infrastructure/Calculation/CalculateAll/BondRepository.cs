using Domain.BondAggreagte;
using Domain.BondAggreagte.Repositories;
using Domain.BondAggreagte.ValueObjects;
using Infrastructure.Common;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Calculation.CalculateAll;

public sealed class BondRepository : IBondRepository
{
    private readonly DataContext _db;

    private readonly IMapper _mapper;

    public BondRepository(DataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<int> CountAsync(CancellationToken token = default)
    {
        return await _db.Bonds.CountAsync(token);
    }

    public async Task UpdateRating(BondId id, int rating, CancellationToken token = default)
    {
        await _db.Bonds.Where(x => x.Id == id.Id)
        .ExecuteUpdateAsync(x => x.SetProperty(x => x.Rating, rating), cancellationToken: token);
    }

    public async Task<List<Bond>> TakeRangeAsync(Range range, CancellationToken token = default)
    {
        var dbBonds = await _db.Bonds.Take(range).ToListAsync(cancellationToken: token);

        return _mapper.Map<List<Bond>>(dbBonds);
    }

    public async Task UpdateCoupons(IEnumerable<Coupon> coupons, BondId id, CancellationToken token = default)
    {
        await _db.Coupons.Where(x => x.BondId == id.Id).ExecuteDeleteAsync(token);

        var dbCoupons = _mapper.Map<IEnumerable<Infrastructure.Common.Models.Coupon>>(coupons);

        await _db.Coupons.AddRangeAsync(dbCoupons, token);
        await _db.SaveChangesAsync(token);
    }

    public async Task<List<Bond>> GetAllFloatingAsync(CancellationToken token = default)
    {
        var dbBonds = await _db.Bonds.Where(x => !x.Coupons.Any(x => x.IsFloating))
                                                .ToListAsync(cancellationToken: token);

        return _mapper.Map<List<Bond>>(dbBonds);
    }

    public async Task RefreshAsync(IEnumerable<Ticker> newBondTickers, CancellationToken token = default)
    {
        await _db.Bonds.Where(x => !newBondTickers.Select(x => x.Value).Contains(x.Ticker))
                       .ExecuteDeleteAsync(cancellationToken: token);
    }

    public async Task<List<Ticker>> UpdateIncomesAsync(IEnumerable<KeyValuePair<Ticker, StaticIncome>> bonds, CancellationToken token = default)
    {
        var notFoundTickers = new List<Ticker>();
        foreach (var (key, value) in bonds)
        {
            var dbBond = await _db.Bonds.FirstOrDefaultAsync(x => x.Ticker == key.ToString(), cancellationToken: token);

            if (dbBond is null)
            {
                notFoundTickers.Add(key);
                continue;
            }

            dbBond.NominalPercent = value.NominalPercent;
            dbBond.AbsoluteNominal = value.AbsoluteNominal;
            dbBond.PricePercent = value.PricePercent;
            dbBond.AbsolutePrice = value.AbsolutePrice;

            await _db.SaveChangesAsync(token);
        }
        return notFoundTickers;
    }

    public async Task<List<Bond>> GetPriceSortedAsync(CancellationToken token = default)
    {
        var bonds = await _db.Bonds.Include(x => x.Coupons)
                                   .OrderBy(x => x.PricePercent)
                                   .ToListAsync(token);

        return _mapper.Map<List<Bond>>(bonds);
    }

    public async Task AddAsync(IEnumerable<Bond> bonds, CancellationToken token = default)
    {
        var dbBonds = _mapper.Map<IEnumerable<Infrastructure.Common.Models.Bond>>(bonds);

        await _db.AddRangeAsync(dbBonds, token);
        await _db.SaveChangesAsync(token);
    }
}
