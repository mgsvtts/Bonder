using Domain.UserAggregate.ValueObjects;
using System.Security.Claims;

namespace Domain.UserAggregate.Repositories;

public interface IUserRepository
{
    Task RegisterAsync(User user, string password);

    Task<User?> GetByUserNameAsync(UserName userName, CancellationToken token);

    Task<bool> IsValidUserAsync(UserName userName, string password, CancellationToken token);

    Task SetRefreshTokenAsync(UserName userName, string refreshToken, CancellationToken token);

    Task<User> AddClaimsAsync(UserName userName, IEnumerable<Claim> claims, CancellationToken token);

    Task<User> RemoveClaimsAsync(UserName userName, IEnumerable<string> claims, CancellationToken token);

    Task<User> DeleteAsync(UserId id, CancellationToken token);
    Task<User?> GetByIdAsync(UserId userId, CancellationToken token);
}