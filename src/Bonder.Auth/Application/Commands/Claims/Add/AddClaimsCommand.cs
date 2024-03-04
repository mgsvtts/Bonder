using Domain.UserAggregate;
using Domain.UserAggregate.ValueObjects;
using Mediator;
using System.Security.Claims;

namespace Application.Commands.Claims.Add;

public sealed record AddClaimsCommand(UserName RequestedBy, UserName AddTo, IEnumerable<Claim> Claims) : ICommand<User>;