using Application.Common;
using Domain.Exceptions;
using Domain.UserAggregate.Repositories;
using Domain.UserAggregate.ValueObjects;
using MediatR;

namespace Application.Refresh;

public class RefreshTokensCommandHandler : IRequestHandler<RefreshTokensCommand, Tokens>
{
    private readonly IJWTTokenGenerator _tokenGenerator;
    private readonly IUserRepository _userRepository;

    public RefreshTokensCommandHandler(IJWTTokenGenerator tokenGenerator, IUserRepository userRepository)
    {
        _tokenGenerator = tokenGenerator;
        _userRepository = userRepository;
    }

    public async Task<Tokens> Handle(RefreshTokensCommand request, CancellationToken cancellationToken)
    {
        var principal = await _tokenGenerator.GetPrincipalFromTokenAsync(request.ExpiredTokens.AccessToken);
        var username = new UserName(principal.Identity?.Name);

        var saved = await _userRepository.GetByUserNameAsync(username, cancellationToken);

        var newTokens = _tokenGenerator.Generate(username);

        if (saved?.Tokens.RefreshToken != request.ExpiredTokens.RefreshToken ||
            newTokens is null)
        {
            throw new AuthorizationException("Invalid info");
        }

        await _userRepository.SetRefreshTokenAsync(username, newTokens.Value.RefreshToken, cancellationToken);

        return newTokens.Value;
    }
}