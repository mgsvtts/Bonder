using Domain.UserAggregate;
using Domain.UserAggregate.Repositories;
using Mediator;

namespace Application.Queries.GetUserByUserName;

public sealed class GetUserByUserNameQueryHandler : IQueryHandler<GetUserByUserNameQuery, User>
{
    private readonly IUserRepository _userRepository;

    public GetUserByUserNameQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async ValueTask<User> Handle(GetUserByUserNameQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetByUserNameAsync(request.UserName, cancellationToken);
    }
}