using Domain.UserAggregate.ValueObjects.Users;
using Mediator;

namespace Application.Commands.AttachTinkoffToken;
public sealed record AttachTinkoffTokenCommand(UserId UserId, TinkoffToken Token) : ICommand;