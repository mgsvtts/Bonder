using Domain.UserAggregate.ValueObjects;
using System.Security.Claims;

namespace Domain.UserAggregate.Repositories;

public interface IUserRepository
{
    Task RegisterAsync(User user, string password);

    Task<User?> GetByUserNameAsync(UserName userName, CancellationToken token = default);

    Task<bool> IsValidUserAsync(UserName userName, string password, CancellationToken token = default);

    Task SetRefreshTokenAsync(UserName userName, string refreshToken, CancellationToken token = default);

    Task<User> AddClaimsAsync(UserName userName, IEnumerable<Claim> claims, CancellationToken token = default);

    Task<User> RemoveClaimsAsync(UserName userName, IEnumerable<string> claims, CancellationToken token = default);

    Task<User> DeleteAsync(UserId id, CancellationToken token = default);
}