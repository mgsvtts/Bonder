using Domain.UserAggregate.ValueObjects;
using Mediator;

namespace Application.Commands.Refresh;

public sealed record RefreshTokensCommand(Tokens ExpiredTokens) : ICommand<Tokens>;