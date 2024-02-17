using Domain.UserAggregate;
using Domain.UserAggregate.ValueObjects;
using MediatR;
using System.Security.Claims;

namespace Application.Claims.Add;

public sealed record AddClaimsCommand(UserName RequestedBy, UserName AddTo, IEnumerable<Claim> Claims) : IRequest<User>;