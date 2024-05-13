using Domain.UserAggregate.ValueObjects;
using Mediator;
using Shared.Domain.Common.ValueObjects;

namespace Application.Commands.Login;

public sealed record LoginCommand(ValidatedString UserName, string Password) : ICommand<Tokens>;