using Domain.BondAggreagte.ValueObjects;

namespace Application.Calculation.Common.Interfaces;

public interface IDohodHttpClient
{
    Task<int?> GetBondRatingAsync(Isin isin, CancellationToken token = default);
}