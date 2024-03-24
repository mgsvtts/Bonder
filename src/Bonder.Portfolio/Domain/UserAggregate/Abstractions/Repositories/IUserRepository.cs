using Domain.UserAggregate.ValueObjects.Users;

namespace Domain.UserAggregate.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User> GetOrCreateByIdAsync(UserId id, CancellationToken token);

    Task SaveAsync(User user, CancellationToken token);

    Task DeleteAsync(UserId id, CancellationToken token);
}