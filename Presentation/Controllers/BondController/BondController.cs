using Application.CalculateBonds;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.BondController.CalculateJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers.BondController;

[Route("api/calculator")]
public class BondController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public BondController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpPost("tickers")]
    public async Task CalculateTickers(IEnumerable<string> tickers, CancellationToken token)
    {
        await _sender.Send(tickers, token);
    }

    [HttpPost("bonds")]
    public async Task<IActionResult> CalculateJson([FromBody] CalculateJsonRequest request, CancellationToken token)
    {
        var result = await _sender.Send(_mapper.Map<CalculateBondsCommand>(request), token);

        return Ok(_mapper.Map<CalculateJsonResponse>(result));
    }
}
