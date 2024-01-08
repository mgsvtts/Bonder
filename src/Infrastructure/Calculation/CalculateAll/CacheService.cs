using Domain.BondAggreagte;
using Domain.BondAggreagte.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Calculation.CalculateAll;
public sealed class CacheService
{
    private readonly IBondRepository _repository;

    public CacheService(IBondRepository repository)
    {
        _repository = repository;
    }

    public async Task CacheAsync(IEnumerable<Bond> bonds, CancellationToken token = default)
    {
        var repositoryTask = _repository.AddOrUpateAsync(bonds, token);

        await Task.WhenAll(repositoryTask);
    }
}
