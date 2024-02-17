using Domain.Exceptions;
using Domain.UserAggregate.Repositories;
using Domain.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace Application.Claims.Remove;
public class RemoveClaimsCommandHandler : IRequestHandler<RemoveClaimsCommand, User>
{
    private readonly IUserRepository _userRepository;

    public RemoveClaimsCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> Handle(RemoveClaimsCommand request, CancellationToken cancellationToken)
    {
        var requestedBy = await _userRepository.GetByUserNameAsync(request.RequestedBy, cancellationToken);

        if (!requestedBy.IsAdmin)
        {
            throw new AuthorizationException("You must be an admin to delete claims");
        }

        request = request with { Claims = request.Claims.Distinct() };

        return await _userRepository.RemoveClaimsAsync(request.AddTo, request.Claims, cancellationToken);
    }
}