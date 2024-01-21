using Domain.BondAggreagte.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common.Models;

[Table("bonds")]
public sealed class Bond
{
    [Key]
    [Column("id")]
    public required Guid Id { get; set; }

    [Column("ticker")]
    public required string Ticker { get; set; }

    [Column("isin")]
    public required string Isin { get; set; }

    [Column("name")]
    public required string Name { get; set; }

    [Column("nominal_income")]
    public required decimal NominalIncome { get; set; }

    [Column("price")]
    public required decimal Price { get; set; }

    [Column("maturity_date")]
    public required DateTime MaturityDate { get; set; }

    [Column("offer_date")]
    public required DateTime? OfferDate { get; set; }

    [Column("rating")]
    public required int? Rating { get; set; }

    public required List<Coupon> Coupons { get; set; }
}
