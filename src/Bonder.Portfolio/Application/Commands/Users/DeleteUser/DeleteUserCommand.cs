using Domain.UserAggregate.ValueObjects.Users;
using Mediator;

namespace Application.Commands.Users.DeleteUser;
public sealed record DeleteUserCommand(UserId UserName) : ICommand;