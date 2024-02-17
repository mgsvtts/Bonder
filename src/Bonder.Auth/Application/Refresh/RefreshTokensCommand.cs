using Domain.UserAggregate.ValueObjects;
using MediatR;

namespace Application.Refresh;

public record RefreshTokensCommand(Tokens ExpiredTokens) : IRequest<Tokens>;