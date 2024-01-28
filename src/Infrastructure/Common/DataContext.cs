using Infrastructure.Common.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Infrastructure.Common;

public sealed class DataContext : DbContext
{
    public DbSet<Bond> Bonds { get; set; }
    public DbSet<Coupon> Coupons{ get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Bond>()
               .HasIndex(u => u.Ticker)
               .HasDatabaseName("ix_ticker_index");

        builder.Entity<Bond>()
              .HasIndex(u => u.Isin)
              .HasDatabaseName("ix_isin_index");

        builder.Entity<Coupon>()
              .HasIndex(u => u.BondId)
              .HasDatabaseName("ix_coupon_bond_id");
    }
}
