using Domain.UserAggregate.ValueObjects;
using Mediator;

namespace Application.AttachTinkoffToken;
public sealed record AttachTinkoffTokenCommand(UserName UserName, string Token) : ICommand;