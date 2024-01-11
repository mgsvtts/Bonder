using Domain.BondAggreagte;
using Domain.BondAggreagte.Repositories;
using Domain.BondAggreagte.ValueObjects;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Calculation.CalculateAll;
public sealed class BondCache : IBondCache
{
    private readonly IDistributedCache _cache;
    private const string _prefix = "bond-";

    public BondCache(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<Bond?> GetByIdAsync(BondId id, CancellationToken token = default)
    {
        var stringBond = await _cache.GetStringAsync(_prefix + id.Ticker.Value, token: token);

        if (stringBond is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<Bond>(stringBond);
    }

    public async Task AddOrUpdateAsync(IEnumerable<Bond> bonds, CancellationToken token = default)
    {
        var tasks = bonds.Select(x => _cache.SetStringAsync(_prefix + x.Identity.Ticker.Value, JsonSerializer.Serialize(x), token));

        await Task.WhenAll(tasks);
    }
}
