using Domain.UserAggregate.ValueObjects.Users;

namespace Domain.UserAggregate.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User> GetByIdAsync(UserId id, CancellationToken token = default);

    Task SaveAsync(User user, CancellationToken token = default);

    Task DeleteAsync(UserId id, CancellationToken token = default);
}