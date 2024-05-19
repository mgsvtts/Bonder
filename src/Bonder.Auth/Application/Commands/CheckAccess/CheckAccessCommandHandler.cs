using Application.Commands.CheckAccess.Dto;
using Domain.Common.Abstractions;
using Domain.UserAggregate;
using Domain.UserAggregate.Repositories;
using Domain.UserAggregate.ValueObjects;
using Mediator;

namespace Application.Commands.CheckAccess;
public sealed class CheckAccessCommandHandler : ICommandHandler<CheckAccessCommand, AccessResult>
{
    private readonly Dictionary<string, Route> _routes;
    private readonly IJWTTokenGenerator _tokenGenerator;
    private readonly IUserRepository _userRepository;

    public CheckAccessCommandHandler(Dictionary<string, Route> routes, IJWTTokenGenerator tokenGenerator, IUserRepository userRepository)
    {
        _routes = routes;
        _tokenGenerator = tokenGenerator;
        _userRepository = userRepository;
    }

    public async ValueTask<AccessResult> Handle(CheckAccessCommand request, CancellationToken token)
    {
        var (tokenExpired, user) = await GetUserAsync(request, token);

        if (string.IsNullOrEmpty(request.Path) || !_routes.TryGetValue(request.Path, out var path))
        {
            return AccessResult.Allowed(user?.Identity, tokenExpired);
        }

        if ((path.Authorized || path.Claims.Any()) && string.IsNullOrEmpty(request.AccessToken))
        {
            return AccessResult.NotAllowed(tokenExpired);
        }

        if (user is null)
        {
            return AccessResult.NotAllowed(tokenExpired);
        }

        if (path.Claims is not null && !path.Claims.All(x => user.Claims.Select(x => x.Value).Contains(x)))
        {
            return AccessResult.NotAllowed(tokenExpired);
        }

        return AccessResult.Allowed(user.Identity);
    }

    private async Task<(bool TokenExpired, User? User)> GetUserAsync(CheckAccessCommand request, CancellationToken token)
    {
        try
        {
            var principal = await _tokenGenerator.ValidateTokenAsync(request.AccessToken, true);

            return (false, await _userRepository.GetByUserNameAsync(new UserName(principal.Identity?.Name), token));
        }
        catch
        {
            return (true, null);
        }
    }
}
