using Domain.UserAggregate.ValueObjects;
using System.Security.Claims;

namespace Application.Common;

public interface IJWTTokenGenerator
{
    Tokens? Generate(string userName);
    Task<ClaimsPrincipal> GetPrincipalFromTokenAsync(string token);
}