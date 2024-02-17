using Application.Common;
using Domain.UserAggregate.ValueObjects;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Token;

public class JwtTokenGenerator : IJWTTokenGenerator
{
    private readonly string _audience;
    private readonly string _issuer;
    private readonly byte[] _key;

    public JwtTokenGenerator(string audience, string issuer, string key)
    {
        _audience = audience;
        _issuer = issuer;
        _key = Encoding.UTF8.GetBytes(key);
    }

    public Tokens? Generate(UserName userName)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new List<Claim>
                {
                   new(ClaimTypes.Name, userName.Name),
                   new(JwtRegisteredClaimNames.Aud, _audience),
                   new(JwtRegisteredClaimNames.Iss, _issuer)
                }),
            Expires = DateTime.Now.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new Tokens(GenerateRefreshToken(), tokenHandler.WriteToken(token));
    }

    public async Task<ClaimsPrincipal> GetPrincipalFromTokenAsync(string? token)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentException($"Token cannot be null or empty", nameof(token));
        }

        var validationParams = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(_key),
            ClockSkew = TimeSpan.Zero
        };

        var result = await new JwtSecurityTokenHandler().ValidateTokenAsync(token, validationParams);

        if (!result.IsValid ||
            result.SecurityToken is not JwtSecurityToken jwtSecurityToken ||
           !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return new ClaimsPrincipal(result.ClaimsIdentity);
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }
}