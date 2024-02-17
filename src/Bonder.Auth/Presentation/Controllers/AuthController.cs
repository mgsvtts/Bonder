using Application.Login;
using Application.Refresh;
using Application.Register;
using Domain.UserAggregate.ValueObjects;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.Dto.Register;

namespace Presentation.Controllers;

[AllowAnonymous]
[Route("api/auth")]
public partial class AuthController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public AuthController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request, CancellationToken token)
    {
        await _sender.Send(_mapper.Map<RegisterCommand>(request), token);

        return NoContent();
    }

    [HttpPost("login")]
    public async Task<Tokens> AuthenticateAsync([FromBody] LoginRequest request, CancellationToken token)
    {
        return await _sender.Send(_mapper.Map<LoginCommand>(request), token);
    }

    [HttpPost("refresh")]
    public async Task<Tokens> Refresh([FromBody] Tokens tokens, CancellationToken token)
    {
        return await _sender.Send(new RefreshTokensCommand(tokens), token);
    }
}