using Application.Login;
using Application.Refresh;
using Application.Register;
using Domain.UserAggregate.ValueObjects;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.Dto.Register;
using Presentation.Filters;

namespace Presentation.Controllers;

[AllowAnonymous]
[ExceptionFilter]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request, CancellationToken token)
    {
        await _sender.Send(request.Adapt<RegisterCommand>(), token);

        return Created();
    }

    [HttpPost("login")]
    public async Task<Tokens> AuthenticateAsync([FromBody] LoginRequest request, CancellationToken token)
    {
        return await _sender.Send(request.Adapt<LoginCommand>(), token);
    }

    [HttpPost("refresh")]
    public async Task<Tokens> Refresh([FromBody] Tokens tokens, CancellationToken token)
    {
        return await _sender.Send(new RefreshTokensCommand(tokens), token);
    }
}