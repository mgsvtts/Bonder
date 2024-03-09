using Domain.UserAggregate;
using Domain.UserAggregate.Repositories;
using Mediator;

namespace Application.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, User?>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async ValueTask<User?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetByIdAsync(request.Id, cancellationToken);
    }
}