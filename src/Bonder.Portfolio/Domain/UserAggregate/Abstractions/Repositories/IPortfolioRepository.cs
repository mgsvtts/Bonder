using Domain.UserAggregate;
using Domain.UserAggregate.ValueObjects;

namespace Domain.UserAggregate.Abstractions.Repositories;
public interface IPortfolioRepository
{
    Task<User> AttachToken(UserName userName, string token, CancellationToken cancellationToken = default);
}