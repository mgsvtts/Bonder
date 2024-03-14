using Domain.Exceptions;
using Domain.UserAggregate;
using Domain.UserAggregate.Repositories;
using Mediator;

namespace Application.Commands.Claims.Remove;

public sealed class RemoveClaimsCommandHandler : ICommandHandler<RemoveClaimsCommand, User>
{
    private readonly IUserRepository _userRepository;

    public RemoveClaimsCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async ValueTask<User> Handle(RemoveClaimsCommand request, CancellationToken token)
    {
        var requestedBy = await _userRepository.GetByUserNameAsync(request.RequestedBy, token)
        ?? throw new UserNotFoundException(request.RequestedBy.ToString());

        if (!requestedBy.IsAdmin)
        {
            throw new AuthorizationException("You must be an admin to delete claims");
        }

        request = request with { Claims = request.Claims.Distinct() };

        return await _userRepository.RemoveClaimsAsync(request.AddTo, request.Claims, token);
    }
}