using Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
