using Domain.UserAggregate;
using Domain.UserAggregate.ValueObjects;
using Mediator;
using System.Security.Claims;

namespace Application.Commands.Claims.Add;

public sealed record AddClaimsCommand(UserId RequestedBy, UserName AddTo, IEnumerable<Claim> Claims) : ICommand<User>;