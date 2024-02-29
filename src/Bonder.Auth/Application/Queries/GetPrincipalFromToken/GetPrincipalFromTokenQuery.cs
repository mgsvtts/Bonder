using MediatR;
using System.Security.Claims;

namespace Application.Queries.GetPrincipalFromToken;
public sealed record GetPrincipalFromTokenQuery(string IdentityToken) : IRequest<ClaimsPrincipal>;