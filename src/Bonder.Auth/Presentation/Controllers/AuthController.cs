using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Login;
using Application.Refresh;
using Application.Register;
using Domain.UserAggregate.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.Register;

namespace Presentation.Controllers
{
    [Authorize]
    [Route("api/auth")]
    public partial class UsersController : ControllerBase
    {
        private readonly ISender _sender;

        public UsersController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public List<string> Get()
        {
            var users = new List<string>
        {
            "Satinder Singh",
            "Amit Sarna",
            "Davin Jon"
        };

            return users;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task RegisterAsync([FromBody] RegisterRequest request, CancellationToken token)
        {
            await _sender.Send(new RegisterCommand(request.UserName, request.Password, request.Email), token);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<Tokens> AuthenticateAsync([FromBody] LoginRequest request, CancellationToken token)
        {
            return await _sender.Send(new LoginCommand(request.UserName, request.Password));
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<Tokens> Refresh([FromBody] Tokens tokens, CancellationToken token)
        {
            return await _sender.Send(new RefreshTokensCommand(tokens));
        }
    }
}