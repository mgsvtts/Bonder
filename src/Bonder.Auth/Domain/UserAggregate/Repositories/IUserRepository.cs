using Domain.UserAggregate.ValueObjects;
using System.Security.Claims;

namespace Domain.UserAggregate.Repositories;

public interface IUserRepository
{
    Task RegisterAsync(User user, string password);

    Task<User> GetByUserNameAsync(UserName userName, CancellationToken cancellationToken = default);

    Task<bool> IsValidUserAsync(UserName userName, string password, CancellationToken cancellationToken = default);

    Task SetRefreshTokenAsync(UserName userName, string refreshToken, CancellationToken cancellationToken = default);

    Task<User> AddClaimsAsync(UserName userName, IEnumerable<Claim> claims, CancellationToken cancellationToken = default);
}