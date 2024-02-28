using Domain.UserAggregate.ValueObjects;
using MediatR;

namespace Application.Commands.Login;

public sealed record LoginCommand(UserName UserName, string Password) : IRequest<Tokens>;