using Domain.UserAggregate.ValueObjects;
using Mediator;

namespace Application.Commands.DeleteUser;
public sealed record DeleteUserCommand(UserId UserId) : ICommand;