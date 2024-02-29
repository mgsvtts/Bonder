using Domain.UserAggregate.ValueObjects;
using MediatR;

namespace Application.DeleteUser;
public sealed record DeleteUserCommand(UserName UserName) : IRequest;