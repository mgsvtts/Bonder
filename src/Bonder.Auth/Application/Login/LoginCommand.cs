using Domain.UserAggregate.ValueObjects;
using MediatR;

namespace Application.Login;

public sealed record LoginCommand(UserName UserName, string Password) : IRequest<Tokens>;