using Domain.UserAggregate.ValueObjects;
using Mediator;

namespace Application.AttachTinkoffToken;
public sealed record AttachTinkoffTokenCommand(UserId UserId, string Token) : ICommand;