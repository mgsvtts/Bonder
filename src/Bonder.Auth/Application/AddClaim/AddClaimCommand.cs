using Domain.UserAggregate;
using Domain.UserAggregate.ValueObjects;
using MediatR;
using System.Security.Claims;

namespace Application.AddClaim;

public sealed record AddClaimCommand(UserName RequestedBy, UserName AddTo, IEnumerable<Claim> Claims) : IRequest<User>;