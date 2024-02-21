using Domain.UserAggregate.ValueObjects.Portfolios;

namespace Application.Common.Abstractions;

public interface ITinkoffHttpClient
{
    Task<IEnumerable<Portfolio>> GetPortfoliosAsync(string token, CancellationToken cancellationToken = default);
}