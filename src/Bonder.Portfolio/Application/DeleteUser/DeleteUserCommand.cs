using Domain.UserAggregate.ValueObjects;
using Mediator;

namespace Application.DeleteUser;
public sealed record DeleteUserCommand(UserName UserName) : ICommand;