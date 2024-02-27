using Domain.Exceptions;
using Domain.UserAggregate;
using Domain.UserAggregate.Repositories;
using MediatR;

namespace Application.Claims.Remove;

public sealed class RemoveClaimsCommandHandler : IRequestHandler<RemoveClaimsCommand, User>
{
    private readonly IUserRepository _userRepository;

    public RemoveClaimsCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> Handle(RemoveClaimsCommand request, CancellationToken cancellationToken)
    {
        var requestedBy = await _userRepository.GetByUserNameAsync(request.RequestedBy, cancellationToken)
        ?? throw new UserNotFoundException(request.RequestedBy.ToString());

        if (!requestedBy.IsAdmin)
        {
            throw new AuthorizationException("You must be an admin to delete claims");
        }

        request = request with { Claims = request.Claims.Distinct() };

        return await _userRepository.RemoveClaimsAsync(request.AddTo, request.Claims, cancellationToken);
    }
}