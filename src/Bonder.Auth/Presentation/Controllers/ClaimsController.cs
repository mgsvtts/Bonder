using Application.Claims.Add;
using Application.Claims.Remove;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.Dto.AddClaims;
using Presentation.Controllers.Dto.RemoveClaims;
using Presentation.Filters;

namespace Presentation.Controllers;

[Authorize]
[ExceptionFilter]
[Route("api/claims")]
public sealed class ClaimsController : ControllerBase
{
    private readonly ISender _sender;

    public ClaimsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<AddClaimResponse> AddClaims([FromBody] AddClaimRequest request, CancellationToken token)
    {
        var response = await _sender.Send((User.Identity.Name, request).Adapt<AddClaimsCommand>(), token);

        return response.Adapt<AddClaimResponse>();
    }

    [HttpDelete]
    public async Task<AddClaimResponse> RemoveClaims([FromBody] RemoveClaimRequest request, CancellationToken token)
    {
        var response = await _sender.Send((User.Identity.Name, request).Adapt<RemoveClaimsCommand>(), token);

        return response.Adapt<AddClaimResponse>();
    }
}