using Domain;

namespace Infrastructure;

public interface IUserRepository
{
    Task<bool> RegisterAsync(User user, string password);
    Task AddUserRefreshTokensAsync(User user);
    Task DeleteUserRefreshTokensAsync(string username, string refreshToken);
    Task<User> GetSavedRefreshTokensAsync(string username, string refreshToken);
    Task<bool> IsValidUserAsync(ValidateUserRequest request);
    Task<int> SaveChangesAsync();
}