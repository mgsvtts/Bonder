using System.Security.Claims;

namespace Infrastructure;

public interface IJwtTokenManager
{
    Tokens? GenerateJWTTokens(string userName);
    string GenerateRefreshToken();
    Tokens GenerateRefreshToken(string username);
    Tokens GenerateToken(string userName);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}