using Domain.UserAggregate;
using Domain.UserAggregate.ValueObjects;
using Mediator;
using Shared.Domain.Common.ValueObjects;
using System.Security.Claims;

namespace Application.Commands.Claims.Add;

public sealed record AddClaimsCommand(UserId RequestedBy, ValidatedString AddTo, IEnumerable<Claim> Claims) : ICommand<User>;