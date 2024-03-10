using Domain.UserAggregate.ValueObjects.Users;
using Mediator;

namespace Application.Commands.DeleteUser;
public sealed record DeleteUserCommand(UserId UserName) : ICommand;