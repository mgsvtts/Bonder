using Application.Common;
using MediatR;
using System.Security.Claims;

namespace Application.Queries.GetPrincipalFromToken;

public sealed class GetPrincipalFromTokenQueryHandler : IRequestHandler<GetPrincipalFromTokenQuery, ClaimsPrincipal>
{
    private readonly IJWTTokenGenerator _tokenGenerator;

    public GetPrincipalFromTokenQueryHandler(IJWTTokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
    }

    public async Task<ClaimsPrincipal> Handle(GetPrincipalFromTokenQuery request, CancellationToken cancellationToken)
    {
        return await _tokenGenerator.ValidateTokenAsync(request.IdentityToken, true);
    }
}