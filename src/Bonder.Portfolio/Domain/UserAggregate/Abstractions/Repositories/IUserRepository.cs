using Domain.UserAggregate.ValueObjects;

namespace Domain.UserAggregate.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User> GetByUserNameAsync(UserName userName, CancellationToken token = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
}