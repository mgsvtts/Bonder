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
        _db.UpdateRange(_mapper.Map<IEnumerable<Infrastructure.Common.Models.Bond>>(bonds));

        await _db.SaveChangesAsync(token);
    }
}
