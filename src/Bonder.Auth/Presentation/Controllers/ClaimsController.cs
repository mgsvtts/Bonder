using Application.Claims.Add;
using Application.Claims.Remove;
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
public class ClaimsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public ClaimsController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> AddClaims([FromBody] AddClaimRequest request, CancellationToken token)
    {
        var response = await _sender.Send(_mapper.Map<AddClaimsCommand>((User.Identity.Name, request)), token);

        return Ok(_mapper.Map<AddClaimResponse>(response));
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveClaims([FromBody] RemoveClaimRequest request, CancellationToken token)
    {
        var response = await _sender.Send(_mapper.Map<RemoveClaimsCommand>((User.Identity.Name, request)), token);

        return Ok(_mapper.Map<AddClaimResponse>(response));
    }
}