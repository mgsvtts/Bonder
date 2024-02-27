using Application.Common;
using Domain.Exceptions;
using Domain.UserAggregate.Repositories;
using Domain.UserAggregate.ValueObjects;
using MediatR;

namespace Application.Refresh;

public sealed class RefreshTokensCommandHandler : IRequestHandler<RefreshTokensCommand, Tokens>
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
        var principal = await _tokenGenerator.ValidateTokenAsync(request.ExpiredTokens.AccessToken);
        var userName = new UserName(principal.Identity?.Name);

        var saved = await _userRepository.GetByUserNameAsync(userName, cancellationToken)
        ?? throw new UserNotFoundException(userName.ToString());

        var newTokens = _tokenGenerator.Generate(userName);

        if (saved?.Tokens.RefreshToken != request.ExpiredTokens.RefreshToken ||
            newTokens is null)
        {
            throw new AuthorizationException("Invalid info");
        }

        await _userRepository.SetRefreshTokenAsync(userName, newTokens.Value.RefreshToken, cancellationToken);

        return newTokens.Value;
    }
}