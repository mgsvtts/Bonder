using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Exceptions;
using Domain.UserAggregate;
using Domain.UserAggregate.Repositories;
using Domain.UserAggregate.Repositories.Dto;
using Infrastructure.Common;
using Infrastructure.Common.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Users;
public class UserRepository : IUserRepository
{
    private readonly DatabaseContext _db;
    private readonly IMapper _mapper;
    private readonly UserManager<Common.Models.User> _userManager;

    public UserRepository(UserManager<Common.Models.User> userManager, DatabaseContext db, IMapper mapper)
    {
        _userManager = userManager;
        _db = db;
        _mapper = mapper;
    }

    public async Task<RegisterResponse> RegisterAsync(Domain.UserAggregate.User user, string password)
    {
        var result = await _userManager.CreateAsync(_mapper.Map<Common.Models.User>(user), password);

        return new RegisterResponse(result.Succeeded, result.Errors.Select(x => x.Description));
    }

    public async Task SetRefreshTokenAsync(string userName, string refreshToken, CancellationToken cancellationToken = default)
    {
        await _db.Users
        .Where(x => x.UserName == userName)
        .ExecuteUpdateAsync(call => call.SetProperty(x => x.RefreshToken, refreshToken), cancellationToken: cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<Domain.UserAggregate.User?> GetByUserNameAndTokenAsync(string userName, string refreshToken, CancellationToken cancellationToken = default)
    {
        var dbUser = await _db.Users.FirstOrDefaultAsync(x => x.UserName == userName && x.RefreshToken == refreshToken, cancellationToken: cancellationToken);

        return _mapper.Map<Domain.UserAggregate.User>(dbUser);
    }

    public async Task<bool> IsValidUserAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(o => o.UserName == userName, cancellationToken: cancellationToken)
        ?? throw new UserNotFoundException(userName);

        return await _userManager.CheckPasswordAsync(user, password);
    }
}
