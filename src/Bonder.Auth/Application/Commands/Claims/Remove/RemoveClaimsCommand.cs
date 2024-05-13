using Domain.UserAggregate;
using Domain.UserAggregate.ValueObjects;
using Mediator;
using Shared.Domain.Common.ValueObjects;

namespace Application.Commands.Claims.Remove;
public sealed record RemoveClaimsCommand(ValidatedString RequestedBy, ValidatedString AddTo, IEnumerable<string> Claims) : ICommand<User>;