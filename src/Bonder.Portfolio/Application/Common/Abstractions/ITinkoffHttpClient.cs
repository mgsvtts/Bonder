using Domain.UserAggregate.ValueObjects.Portfolios;

namespace Application.Common.Abstractions;

public interface ITinkoffHttpClient
{
    Task<List<Portfolio>> GetPortfoliosAsync(string token, CancellationToken cancellationToken = default);
}