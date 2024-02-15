using Domain.UserAggregate.Repositories.Dto;

namespace Domain.UserAggregate.Repositories;

public interface IUserRepository
{
    Task<bool> IsValidUserAsync(string userName, string password, CancellationToken cancellationToken = default);
    Task SetRefreshTokenAsync(string userName, string refreshToken, CancellationToken cancellationToken = default);
    Task<User?> GetByUserNameAndTokenAsync(string username, string refreshToken, CancellationToken cancellationToken = default);
    Task<RegisterResponse> RegisterAsync(User user, string password);
}