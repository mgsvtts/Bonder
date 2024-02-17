using Domain.Exceptions;
using Domain.UserAggregate;
using Domain.UserAggregate.Repositories;
using MediatR;

namespace Application.AddClaim;

public sealed class AddClaimCommandHandler : IRequestHandler<AddClaimCommand, User>
{
    private readonly IUserRepository _userRepository;

    public AddClaimCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> Handle(AddClaimCommand request, CancellationToken cancellationToken)
    {
        var requestedBy = await _userRepository.GetByUserNameAsync(request.RequestedBy, cancellationToken);

        if (!requestedBy.IsAdmin)
        {
            throw new AuthorizationException("You must be an admin to set claims");
        }

        return await _userRepository.AddClaimsAsync(request.AddTo, request.Claims, cancellationToken);
    }
}