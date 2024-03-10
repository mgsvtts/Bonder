using Domain.UserAggregate.ValueObjects.Users;

namespace Application.Common.Abstractions;

public interface IUserBuilder
{
    public Task<Domain.UserAggregate.User> BuildAsync(UserId userName, TinkoffToken tinkoffToken, CancellationToken cancellationToken = default);
}