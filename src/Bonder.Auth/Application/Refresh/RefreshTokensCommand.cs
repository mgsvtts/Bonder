using Domain.UserAggregate.ValueObjects;
using MediatR;

namespace Application.Refresh;

public sealed record RefreshTokensCommand(Tokens ExpiredTokens) : IRequest<Tokens>;