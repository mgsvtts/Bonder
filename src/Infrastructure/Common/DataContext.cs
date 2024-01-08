using Infrastructure.Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common;
public sealed class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Bond>().HasKey(table => new {
            table.Ticker,
            table.Isin
        });
    }

    public DbSet<Bond> Bonds { get; set; }

}
