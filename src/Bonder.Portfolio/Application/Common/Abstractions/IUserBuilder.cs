using Domain.UserAggregate.ValueObjects;

namespace Application.Common.Abstractions;

public interface IUserBuilder
{
    public Task<Domain.UserAggregate.User> BuildAsync(UserName userName, string tinkoffToken, CancellationToken cancellationToken = default);
}