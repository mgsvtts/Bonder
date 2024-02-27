using Infrastructure.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common;

public sealed class DatabaseContext : IdentityDbContext<User>
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().Ignore(c => c.AccessFailedCount)
                              .Ignore(c => c.LockoutEnabled)
                              .Ignore(c => c.TwoFactorEnabled)
                              .Ignore(c => c.ConcurrencyStamp)
                              .Ignore(c => c.EmailConfirmed)
                              .Ignore(c => c.LockoutEnd)
                              .Ignore(c => c.PhoneNumber)
                              .Ignore(c => c.PhoneNumberConfirmed);

        builder.Entity<User>().ToTable("users");
        builder.Entity<IdentityRole>().ToTable("roles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("user_claims");
        builder.Entity<IdentityUserRole<string>>().ToTable("user_roles");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("role_claims");
        builder.Ignore<IdentityUserToken<string>>();
        builder.Ignore<IdentityUserLogin<string>>();
    }
}