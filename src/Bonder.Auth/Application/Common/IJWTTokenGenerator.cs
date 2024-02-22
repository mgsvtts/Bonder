using Domain.UserAggregate.ValueObjects;
using System.Security.Claims;

namespace Application.Common;

public interface IJWTTokenGenerator
{
    Tokens? Generate(UserName userName);

    Task<ClaimsPrincipal> ValidateTokenAsync(string? token, bool allowExpiration = false);
}