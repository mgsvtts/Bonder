using Domain.BondAggreagte;
using Domain.BondAggreagte.Repositories;
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

    public async Task AddOrUpateAsync(IEnumerable<Bond> bonds, CancellationToken token = default)
    {
        var dbBonds = _mapper.Map<IEnumerable<Infrastructure.Common.Models.Bond>>(bonds);

        var toUpdate = await _db.Bonds.Where(x => dbBonds.Select(x => x.Ticker).Contains(x.Ticker))
                                      .ToListAsync(cancellationToken: token);

        var toAdd = dbBonds.ExceptBy(toUpdate.Select(x => x.Ticker), x => x.Ticker);

        _db.UpdateRange(toUpdate);
        await _db.AddRangeAsync(toAdd, token);

        await _db.SaveChangesAsync(token);
    }

    public async Task<List<Bond>> GetPriceSortedBondsAsync(CancellationToken token = default)
    {
        var bonds = await _db.Bonds.Include(x => x.Coupons)
                                   .OrderBy(x => x.Price)
                                   .ToListAsync(token);

        return _mapper.Map<List<Bond>>(bonds);
    }
}
