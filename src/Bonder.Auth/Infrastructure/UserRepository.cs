using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;
public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;
    private readonly DatabaseContext _db;

    public UserRepository(UserManager<User> userManager, DatabaseContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    public async Task<bool> RegisterAsync(User user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);

        return result.Succeeded;
    }
    public async Task SetRefreshTokenAsync(string userName, string refreshToken)
    {
        await _db.Users.Where(x => x.UserName == userName)
        .ExecuteUpdateAsync(call => call.SetProperty(x => x.RefreshToken, refreshToken));
    }

    public Task<User?> GetSavedRefreshTokensAsync(string username, string refreshToken)
    {
        return _db.Users.FirstOrDefaultAsync(x => x.UserName == username && x.RefreshToken == refreshToken);
    }

    public Task<int> SaveChangesAsync()
    {
        return _db.SaveChangesAsync();
    }

    public Task<bool> IsValidUserAsync(ValidateUserRequest request)
    {
        var user = _userManager.Users.FirstOrDefault(o => o.UserName == request.UserName);

        return _userManager.CheckPasswordAsync(user, request.Password);
    }
}
