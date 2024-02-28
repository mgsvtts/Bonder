using Domain.UserAggregate.ValueObjects;
using MediatR;

namespace Application.Commands.Refresh;

public sealed record RefreshTokensCommand(Tokens ExpiredTokens) : IRequest<Tokens>;