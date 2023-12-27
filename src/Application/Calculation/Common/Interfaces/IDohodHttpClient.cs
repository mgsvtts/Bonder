using Domain.BondAggreagte.ValueObjects;

namespace Infrastructure.Calculation;

public interface IDohodHttpClient
{
    Task<int> GetBondRatingAsync(Isin isin, CancellationToken token = default);
}