using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Common.Models;

public sealed class User : IdentityUser
{
    public string? RefreshToken { get; set; }
}