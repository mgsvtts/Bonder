using Domain.UserAggregate.ValueObjects;
using Mediator;

namespace Application.Commands.Login;

public sealed record LoginCommand(UserName UserName, string Password) : ICommand<Tokens>;