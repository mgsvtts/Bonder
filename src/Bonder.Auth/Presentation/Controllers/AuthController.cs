using Application.Commands.DeleteUser;
using Application.Commands.Login;
using Application.Commands.Refresh;
using Application.Commands.Register;
using Domain.UserAggregate.ValueObjects;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public async Task<IResult> RegisterAsync([FromBody] RegisterRequest request, CancellationToken token)
    {
        await _sender.Send(request.Adapt<RegisterCommand>(), token);

        return TypedResults.Created();
    }

    [HttpPost("login")]
    public async Task<Tokens> AuthenticateAsync([FromBody] LoginRequest request, CancellationToken token)
    {
        return await _sender.Send(request.Adapt<LoginCommand>(), token);
    }

    [HttpPost("refresh")]
    public async Task<Tokens> RefreshAsync(Dto.RefreshTokens.Tokens tokens, CancellationToken token)
    {
        return await _sender.Send(new RefreshTokensCommand(tokens.Adapt<Tokens>()), token);
    }

    [HttpDelete("{userId}")]
    public async Task<IResult> DeleteAsync([FromRoute] Guid userId, CancellationToken token)
    {
        await _sender.Send(new DeleteUserCommand(new UserId(userId)), token);

        return TypedResults.NoContent();
    }
}