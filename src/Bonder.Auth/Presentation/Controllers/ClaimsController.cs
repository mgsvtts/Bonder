using Application.AddClaim;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.Dto.AddClaims;

namespace Presentation.Controllers;

[Authorize]
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

    [HttpPost("add")]
    public async Task<IActionResult> AddClaim([FromBody] AddClaimRequest request, CancellationToken token)
    {
        var response = await _sender.Send(_mapper.Map<AddClaimCommand>((User.Identity.Name, request)), token);

        return Ok(_mapper.Map<AddClaimResponse>(response));
    }
}