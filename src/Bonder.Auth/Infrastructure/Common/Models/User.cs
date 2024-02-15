using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Common.Models;

public class User : IdentityUser
{
    public string? RefreshToken { get; set; }
}