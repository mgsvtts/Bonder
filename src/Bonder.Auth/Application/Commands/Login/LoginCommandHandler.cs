using Application.Common;
using Domain.Exceptions;
using Domain.UserAggregate.Repositories;
using Domain.UserAggregate.ValueObjects;
using Mediator;

namespace Application.Commands.Login;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, Tokens>
{
    private readonly IUserRepository _userRepository;
    private readonly IJWTTokenGenerator _tokenGenerator;

    public LoginCommandHandler(IUserRepository userRepository, IJWTTokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
    }

    public async ValueTask<Tokens> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var validUser = await _userRepository.IsValidUserAsync(request.UserName, request.Password, cancellationToken);

        if (!validUser)
        {
            throw new AuthorizationException("Incorrect username or password");
        }

        var token = _tokenGenerator.Generate(request.UserName)
        ?? throw new AuthorizationException("Invalid Attempt!");

        await _userRepository.SetRefreshTokenAsync(request.UserName, token.RefreshToken, cancellationToken);

        return token;
    }
}