using Domain.BondAggreagte.ValueObjects.Identities;

namespace Application.Commands.Calculation.Common.Abstractions;

public interface IDohodHttpClient
{
    Task<int?> GetBondRatingAsync(Isin isin, CancellationToken token = default);
}