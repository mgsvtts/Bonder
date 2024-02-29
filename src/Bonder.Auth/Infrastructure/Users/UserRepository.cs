using Domain.Exceptions;
using Domain.UserAggregate.Repositories;
using Domain.UserAggregate.ValueObjects;
using Infrastructure.Common;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Infrastructure.Users;

public sealed class UserRepository : IUserRepository
{
    private readonly DatabaseContext _db;
    private readonly UserManager<Common.Models.User> _userManager;

    public UserRepository(UserManager<Common.Models.User> userManager, DatabaseContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    public async Task RegisterAsync(Domain.UserAggregate.User user, string password)
    {
        var result = await _userManager.CreateAsync(user.Adapt<Common.Models.User>(), password);

        if (!result.Succeeded)
        {
            throw new AuthorizationException(string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)));
        }
    }

    public async Task<Domain.UserAggregate.User> DeleteAsync(UserId id, CancellationToken token = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id.Identity.ToString(), cancellationToken: token)
        ?? throw new UserNotFoundException(id.Identity.ToString());

        await _db.Users.Where(x => x.Id == id.Identity.ToString())
        .ExecuteDeleteAsync(cancellationToken: token);

        var claims = await _userManager.GetClaimsAsync(user);

        return (user, claims).Adapt<Domain.UserAggregate.User>();
    }

    public async Task SetRefreshTokenAsync(UserName userName, string refreshToken, CancellationToken cancellationToken = default)
    {
        await _db.Users
        .Where(x => x.UserName == userName.Name)
        .ExecuteUpdateAsync(call => call.SetProperty(x => x.RefreshToken, refreshToken), cancellationToken: cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<Domain.UserAggregate.User?> GetByUserNameAsync(UserName userName, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.UserName == userName.Name, cancellationToken: cancellationToken);

        if (user is null)
        {
            return null;
        }

        var claims = await _userManager.GetClaimsAsync(user);

        return (user, claims).Adapt<Domain.UserAggregate.User>();
    }

    public async Task<bool> IsValidUserAsync(UserName userName, string password, CancellationToken cancellationToken = default)
    {
        var user = await GetByUserNameInternalAsync(userName, cancellationToken);

        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<Domain.UserAggregate.User> AddClaimsAsync(UserName userName, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
    {
        var user = await GetByUserNameInternalAsync(userName, cancellationToken);

        var result = await _userManager.AddClaimsAsync(user, claims);

        if (!result.Succeeded)
        {
            throw new AuthorizationException(string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)));
        }

        var actualClaims = await _userManager.GetClaimsAsync(user);

        return (user, actualClaims).Adapt<Domain.UserAggregate.User>();
    }

    public async Task<Domain.UserAggregate.User> RemoveClaimsAsync(UserName userName, IEnumerable<string> claims, CancellationToken cancellationToken = default)
    {
        var user = await GetByUserNameInternalAsync(userName, cancellationToken);

        await _db.UserClaims
        .Where(x => claims.Contains(x.ClaimType))
        .ExecuteDeleteAsync(cancellationToken: cancellationToken);

        var actualClaims = await _userManager.GetClaimsAsync(user);

        return (user, actualClaims).Adapt<Domain.UserAggregate.User>();
    }

    private async Task<Common.Models.User> GetByUserNameInternalAsync(UserName userName, CancellationToken cancellationToken = default)
    {
        return await _db.Users.FirstOrDefaultAsync(x => x.UserName == userName.Name, cancellationToken: cancellationToken)
        ?? throw new UserNotFoundException(userName.Name);
    }
}