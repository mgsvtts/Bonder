using Domain.UserAggregate;
using Domain.UserAggregate.Repositories;
using MediatR;

namespace Application.Queries.GetUserByUserName;

public sealed class GetUserByUserNameQueryHandler : IRequestHandler<GetUserByUserNameQuery, User>
{
    private readonly IUserRepository _userRepository;

    public GetUserByUserNameQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> Handle(GetUserByUserNameQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetByUserNameAsync(request.UserName, cancellationToken);
    }
}