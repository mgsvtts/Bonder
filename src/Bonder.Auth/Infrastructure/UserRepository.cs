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
    public async Task AddUserRefreshTokensAsync(User user)
    {
        await _db.Users.Where(x => x.UserName == user.UserName)
        .ExecuteUpdateAsync(call => call.SetProperty(x => x.RefreshToken, user.RefreshToken));
    }

    public async Task DeleteUserRefreshTokensAsync(string username, string refreshToken)
    {
        var item = await _db.Users.FirstOrDefaultAsync(x => x.UserName == username && x.RefreshToken == refreshToken);
        if (item != null)
        {
            _db.Users.Remove(item);
        }
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
