using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Identities;

namespace Domain.BondAggreagte.Abstractions;

public interface IDohodHttpClient
{
    Task<Rating?> GetBondRatingAsync(Isin isin, CancellationToken token);
}