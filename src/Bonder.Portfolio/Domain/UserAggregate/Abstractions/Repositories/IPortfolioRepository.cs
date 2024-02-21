using Domain.UserAggregate.ValueObjects;

namespace Domain.UserAggregate.Abstractions.Repositories;

public interface IPortfolioRepository
{
    Task AttachToken(UserName userName, string token, CancellationToken cancellationToken = default);

    public Task<string> GetTokenAsync(UserName userName, CancellationToken cancellationToken = default);
}