﻿using Application.Calculation.CalculateTickers;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.BondController.Calculate;
using Presentation.Controllers.BondController.Calculate.Request;

namespace Presentation.Controllers.BondController;

[Route("api/calculate")]
public class BondController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public BondController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> CalculateTickers([FromBody] CalculateRequest request, CancellationToken token)
    {
        var result = await _sender.Send(_mapper.Map<CalculateTickersCommand>(request), token);

        return Ok(result.MapToResponse());
    }
}