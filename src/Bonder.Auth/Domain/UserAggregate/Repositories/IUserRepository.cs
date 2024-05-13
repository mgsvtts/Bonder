using Domain.UserAggregate.ValueObjects;
using Shared.Domain.Common.ValueObjects;
using System.Security.Claims;

namespace Domain.UserAggregate.Repositories;

public interface IUserRepository
{
    Task RegisterAsync(User user, string password);

    Task<User?> GetByUserNameAsync(ValidatedString userName, CancellationToken token);

    Task<bool> IsValidUserAsync(ValidatedString userName, string password, CancellationToken token);

    Task SetRefreshTokenAsync(ValidatedString userName, string refreshToken, CancellationToken token);

    Task<User> AddClaimsAsync(ValidatedString userName, IEnumerable<Claim> claims, CancellationToken token);

    Task<User> RemoveClaimsAsync(ValidatedString userName, IEnumerable<string> claims, CancellationToken token);

    Task<User> DeleteAsync(UserId id, CancellationToken token);
    Task<User?> GetByIdAsync(UserId userId, CancellationToken token);
}