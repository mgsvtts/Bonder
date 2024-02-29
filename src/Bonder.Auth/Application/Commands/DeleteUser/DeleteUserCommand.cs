using Domain.UserAggregate.ValueObjects;
using MediatR;

namespace Application.Commands.DeleteUser;
public sealed record DeleteUserCommand(UserId UserId) : IRequest;