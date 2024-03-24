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

    public async Task<Domain.UserAggregate.User> DeleteAsync(UserId id, CancellationToken token)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id.Value.ToString(), cancellationToken: token)
        ?? throw new UserNotFoundException(id.Value.ToString());

        await _db.Users.Where(x => x.Id == id.Value.ToString())
        .ExecuteDeleteAsync(cancellationToken: token);

        var claims = await _userManager.GetClaimsAsync(user);

        return (user, claims).Adapt<Domain.UserAggregate.User>();
    }

    public async Task SetRefreshTokenAsync(UserName userName, string refreshToken, CancellationToken token)
    {
        await _db.Users
        .Where(x => x.UserName == userName.Name)
        .ExecuteUpdateAsync(call => call.SetProperty(x => x.RefreshToken, refreshToken), cancellationToken: token);

        await _db.SaveChangesAsync(token);
    }

    public async Task<Domain.UserAggregate.User?> GetByUserNameAsync(UserName userName, CancellationToken token)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.UserName == userName.Name, cancellationToken: token);

        if (user is null)
        {
            return null;
        }

        var claims = await _userManager.GetClaimsAsync(user);

        return (user, claims).Adapt<Domain.UserAggregate.User>();
    }

    public async Task<Domain.UserAggregate.User?> GetByIdAsync(UserId userId, CancellationToken token)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId.ToString(), cancellationToken: token);

        if (user is null)
        {
            return null;
        }

        var claims = await _userManager.GetClaimsAsync(user);

        return (user, claims).Adapt<Domain.UserAggregate.User>();
    }

    public async Task<bool> IsValidUserAsync(UserName userName, string password, CancellationToken token)
    {
        var user = await GetByUserNameInternalAsync(userName, token);

        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<Domain.UserAggregate.User> AddClaimsAsync(UserName userName, IEnumerable<Claim> claims, CancellationToken token)
    {
        var user = await GetByUserNameInternalAsync(userName, token);

        var result = await _userManager.AddClaimsAsync(user, claims);

        if (!result.Succeeded)
        {
            throw new AuthorizationException(string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)));
        }

        var actualClaims = await _userManager.GetClaimsAsync(user);

        return (user, actualClaims).Adapt<Domain.UserAggregate.User>();
    }

    public async Task<Domain.UserAggregate.User> RemoveClaimsAsync(UserName userName, IEnumerable<string> claims, CancellationToken token)
    {
        var user = await GetByUserNameInternalAsync(userName, token);

        await _db.UserClaims
        .Where(x => claims.Contains(x.ClaimType))
        .ExecuteDeleteAsync(cancellationToken: token);

        var actualClaims = await _userManager.GetClaimsAsync(user);

        return (user, actualClaims).Adapt<Domain.UserAggregate.User>();
    }

    private async Task<Common.Models.User> GetByUserNameInternalAsync(UserName userName, CancellationToken token)
    {
        return await _db.Users.FirstOrDefaultAsync(x => x.UserName == userName.Name, cancellationToken: token)
        ?? throw new UserNotFoundException(userName.Name);
    }
}