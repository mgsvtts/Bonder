using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Authorize]
    [Route("api/auth")]
    public class UsersController : ControllerBase
    {
        private readonly IJwtTokenManager _tokenManager;
        private readonly IUserRepository _userService;

        public UsersController(IJwtTokenManager tokenManager, IUserRepository userService)
        {
            _tokenManager = tokenManager;
            _userService = userService;
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
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
        {
            var result = await _userService.RegisterAsync(new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = request.Email,
                UserName = request.UserName,
            }, request.Password);

            return Ok(new { Succes = result });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] ValidateUserRequest request)
        {
            var validUser = await _userService.IsValidUserAsync(request);

            if (!validUser)
            {
                return Unauthorized("Incorrect username or password!");
            }

            var token = _tokenManager.GenerateToken(request.UserName);

            if (token == null)
            {
                return Unauthorized("Invalid Attempt!");
            }

            await _userService.SetRefreshTokenAsync(request.UserName, token.RefreshToken);
            await _userService.SaveChangesAsync();

            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] Tokens token)
        {
            var principal = _tokenManager.GetPrincipalFromExpiredToken(token.AccessToken);
            var username = principal.Identity?.Name;

            var savedRefreshToken = await _userService.GetSavedRefreshTokensAsync(username, token.RefreshToken);

            if (savedRefreshToken.RefreshToken != token.RefreshToken)
            {
                return Unauthorized("Invalid attempt!");
            }

            var newJwtToken = _tokenManager.GenerateRefreshToken(username);

            if (newJwtToken == null)
            {
                return Unauthorized("Invalid attempt!");
            }

            var obj = new User
            {
                RefreshToken = newJwtToken.RefreshToken,
                UserName = username
            };

            await _userService.SetRefreshTokenAsync(username, newJwtToken.RefreshToken);
            await _userService.SaveChangesAsync();

            return Ok(newJwtToken);
        }
    }
}