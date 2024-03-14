using Domain.UserAggregate.ValueObjects.Users;

namespace Domain.Common.Abstractions;

public interface IUserBuilder
{
    public Task<UserAggregate.User> BuildAsync(UserId userName, TinkoffToken tinkoffToken, CancellationToken token = default);
}