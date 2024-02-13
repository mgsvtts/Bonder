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
    [Route("api/[controller]")]
    [ApiController]
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

        public record RegisterRequest(string UserName, string Password, string Email);
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            var result = await _userService.RegisterAsync(new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = request.Email,
                UserName = request.UserName,
            }, request.Password);

            return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync(ValidateUserRequest request)
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

            var obj = new User
            {
                RefreshToken = token.RefreshToken,
                UserName = request.UserName
            };

            await _userService.AddUserRefreshTokensAsync(obj);
            await _userService.SaveChangesAsync();

            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(Tokens token)
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

            await _userService.DeleteUserRefreshTokensAsync(username, token.RefreshToken);
            await _userService.AddUserRefreshTokensAsync(obj);
            await _userService.SaveChangesAsync();

            return Ok(newJwtToken);
        }
    }
}