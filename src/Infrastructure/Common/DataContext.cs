﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Application.Calculation.Common.Interfaces;
using Infrastructure.Common.Models;

namespace Infrastructure.Common;
public sealed class DataContext : DbContext
{
    public DbSet<Bond> Bonds { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
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
