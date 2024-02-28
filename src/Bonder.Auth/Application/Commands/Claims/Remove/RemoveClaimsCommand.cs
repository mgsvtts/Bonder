using Domain.UserAggregate;
using Domain.UserAggregate.ValueObjects;
using MediatR;

namespace Application.Commands.Claims.Remove;
public sealed record RemoveClaimsCommand(UserName RequestedBy, UserName AddTo, IEnumerable<string> Claims) : IRequest<User>;