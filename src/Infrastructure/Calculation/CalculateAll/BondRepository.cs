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

    public async Task UpdateAsync(IEnumerable<KeyValuePair<Ticker, StaticIncome>> bonds, CancellationToken token = default)
    {
        foreach(var pair in bonds)
        {
            var dbBond = await _db.Bonds.FirstOrDefaultAsync(x => x.Ticker == pair.Key.ToString(), cancellationToken: token);

            if(dbBond is null)
            {
                continue;
            }

            dbBond.NominalPercent = pair.Value.NominalPercent;
            dbBond.AbsoluteNominal = pair.Value.AbsoluteNominal;
            dbBond.PricePercent = pair.Value.PricePercent;
            dbBond.AbsolutePrice = pair.Value.AbsolutePrice;

            await _db.SaveChangesAsync(token);
        }
    }

    public async Task<List<Bond>> GetPriceSortedBondsAsync(CancellationToken token = default)
    {
        var bonds = await _db.Bonds.Include(x => x.Coupons)
                                   .OrderBy(x => x.PricePercent)
                                   .ToListAsync(token);

        return _mapper.Map<List<Bond>>(bonds);
    }
}
