using Domain.UserAggregate.ValueObjects;
using Shared.Domain.Common.ValueObjects;
using System.Security.Claims;

namespace Domain.Common.Abstractions;

public interface IJWTTokenGenerator
{
    Tokens? Generate(ValidatedString userName);

    Task<ClaimsPrincipal> ValidateTokenAsync(string? token, bool validateLifetime = false);
}