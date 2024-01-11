using Domain.BondAggreagte;
using Domain.BondAggreagte.Repositories;
using Domain.BondAggreagte.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Calculation.CalculateAll;
public sealed class CacheService : ICacheService
{
    private readonly IBondRepository _repository;
    private readonly IBondCache _cache;

    public CacheService(IBondRepository repository, IBondCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task CacheAsync(IEnumerable<Bond> bonds, CancellationToken token = default)
    {
        var repositoryTask = _repository.AddOrUpateAsync(bonds, token);
        var cacheTask = _cache.AddOrUpdateAsync(bonds, token);

        await Task.WhenAll(repositoryTask, cacheTask);
    }
}
