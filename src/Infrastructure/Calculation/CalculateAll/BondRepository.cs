using Domain.BondAggreagte;
using Domain.BondAggreagte.Repositories;
using Infrastructure.Common;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
