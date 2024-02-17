using Domain.Exceptions;
using Domain.UserAggregate.Repositories;
using Domain.UserAggregate.ValueObjects;
using Infrastructure.Common;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Infrastructure.Users;

public class UserRepository : IUserRepository
{
    private readonly IMapper _mapper;
    private readonly DatabaseContext _db;
    private readonly UserManager<Common.Models.User> _userManager;

    public UserRepository(UserManager<Common.Models.User> userManager, DatabaseContext db, IMapper mapper)
    {
        _userManager = userManager;
        _db = db;
        _mapper = mapper;
    }

    public async Task RegisterAsync(Domain.UserAggregate.User user, string password)
    {
        var result = await _userManager.CreateAsync(_mapper.Map<Common.Models.User>(user), password);

        if (!result.Succeeded)
        {
            throw new AuthorizationException(string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)));
        }
    }

    public async Task SetRefreshTokenAsync(UserName userName, string refreshToken, CancellationToken cancellationToken = default)
    {
        await _db.Users
        .Where(x => x.UserName == userName.Name)
        .ExecuteUpdateAsync(call => call.SetProperty(x => x.RefreshToken, refreshToken), cancellationToken: cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<Domain.UserAggregate.User> GetByUserNameAsync(UserName userName, CancellationToken cancellationToken = default)
    {
        var user = await GetByUserNameInternalAsync(userName, cancellationToken);

        var claims = await _userManager.GetClaimsAsync(user);

        return _mapper.Map<Domain.UserAggregate.User>((user, claims));
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

        return _mapper.Map<Domain.UserAggregate.User>((user, actualClaims));
    }

    public async Task<Domain.UserAggregate.User> RemoveClaimsAsync(UserName userName, IEnumerable<string> claims, CancellationToken cancellationToken = default)
    {
        var user = await GetByUserNameInternalAsync(userName, cancellationToken);

        await _db.UserClaims
        .Where(x => claims.Contains(x.ClaimType))
        .ExecuteDeleteAsync(cancellationToken: cancellationToken);

        var actualClaims = await _userManager.GetClaimsAsync(user);

        return _mapper.Map<Domain.UserAggregate.User>((user, actualClaims));
    }

    private async Task<Common.Models.User> GetByUserNameInternalAsync(UserName userName, CancellationToken cancellationToken = default)
    {
        return await _db.Users.FirstOrDefaultAsync(x => x.UserName == userName.Name, cancellationToken: cancellationToken)
        ?? throw new UserNotFoundException(userName.Name);
    }
}