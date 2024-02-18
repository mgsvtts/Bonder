using Domain.Exceptions;
using Domain.UserAggregate;
using Domain.UserAggregate.Repositories;
using MediatR;

namespace Application.Claims.Add;

public sealed class AddClaimsCommandHandler : IRequestHandler<AddClaimsCommand, User>
{
    private readonly IUserRepository _userRepository;

    public AddClaimsCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> Handle(AddClaimsCommand request, CancellationToken cancellationToken)
    {
        var requestedBy = await _userRepository.GetByUserNameAsync(request.RequestedBy, cancellationToken)
        ?? throw new UserNotFoundException(request.RequestedBy.ToString());

        if (!requestedBy.IsAdmin)
        {
            throw new AuthorizationException("You must be an admin to set claims");
        }

        request = request with { Claims = request.Claims.DistinctBy(x => x.Type) };

        var user = await _userRepository.GetByUserNameAsync(request.AddTo, cancellationToken)
        ?? throw new UserNotFoundException(request.AddTo.ToString());

        var existingClaims = request.Claims.Where(x => user.Claims.Select(x => x.Type).Contains(x.Type));
        if (existingClaims.Any())
        {
            throw new InvalidOperationException($"User {request.AddTo.Name} already has claims: {string.Join(", ", existingClaims.Select(x => x.Value))}");
        }

        return await _userRepository.AddClaimsAsync(request.AddTo, request.Claims, cancellationToken);
    }
}