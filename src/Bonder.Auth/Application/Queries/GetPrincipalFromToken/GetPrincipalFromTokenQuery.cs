using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.GetPrincipalFromToken;
public sealed record GetPrincipalFromTokenQuery(string IdentityToken) : IRequest<ClaimsPrincipal>;
