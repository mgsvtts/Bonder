using Application.Common;
using Domain.Common.Abstractions;
using Domain.Exceptions;
using Domain.UserAggregate.Repositories;
using Domain.UserAggregate.ValueObjects;
using Mediator;

namespace Application.Commands.Refresh;

public sealed class RefreshTokensCommandHandler : ICommandHandler<RefreshTokensCommand, Tokens>
{
    private readonly IJWTTokenGenerator _tokenGenerator;
    private readonly IUserRepository _userRepository;

    public RefreshTokensCommandHandler(IJWTTokenGenerator tokenGenerator, IUserRepository userRepository)
    {
        _tokenGenerator = tokenGenerator;
        _userRepository = userRepository;
    }

    public async ValueTask<Tokens> Handle(RefreshTokensCommand request, CancellationToken token)
    {
        var principal = await _tokenGenerator.ValidateTokenAsync(request.ExpiredTokens.AccessToken);
        var userName = new UserName(principal.Identity?.Name);

        var saved = await _userRepository.GetByUserNameAsync(userName, token)
        ?? throw new UserNotFoundException(userName.ToString());

        var newTokens = _tokenGenerator.Generate(userName);

        if (saved?.Tokens.RefreshToken != request.ExpiredTokens.RefreshToken ||
            newTokens is null)
        {
            throw new AuthorizationException("Invalid token info");
        }

        await _userRepository.SetRefreshTokenAsync(userName, newTokens.Value.RefreshToken, token);

        return newTokens.Value;
    }
}