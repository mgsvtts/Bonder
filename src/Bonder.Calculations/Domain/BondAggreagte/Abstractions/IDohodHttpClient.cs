using Domain.BondAggreagte.ValueObjects.Identities;

namespace Domain.BondAggreagte.Abstractions;

public interface IDohodHttpClient
{
    Task<int?> GetBondRatingAsync(Isin isin, CancellationToken token = default);
}