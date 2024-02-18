using Application.AttachTinkoffToken;
using Domain.UserAggregate.ValueObjects;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers;

[ExceptionFilter]
[Route("api/portfolio")]
public sealed class PortfolioController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public PortfolioController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpPost("attach-token")]
    public async Task AttachToken([FromBody] AttachTokenRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(_mapper.Map<AttachTinkoffTokenCommand>((User.Identity.Name, request.Token)), cancellationToken);
    }
}

public sealed record AttachTokenRequest(string Token);