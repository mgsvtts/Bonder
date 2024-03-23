using Domain.Common.Abstractions;
using Mediator;
using System.Security.Claims;

namespace Application.Queries.GetPrincipalFromToken;

public sealed class GetPrincipalFromTokenQueryHandler : IRequestHandler<GetPrincipalFromTokenQuery, ClaimsPrincipal>
{
    private readonly IJWTTokenGenerator _tokenGenerator;

    public GetPrincipalFromTokenQueryHandler(IJWTTokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
    }

    public async ValueTask<ClaimsPrincipal> Handle(GetPrincipalFromTokenQuery request, CancellationToken token)
    {
        return await _tokenGenerator.ValidateTokenAsync(request.IdentityToken, true);
    }
}