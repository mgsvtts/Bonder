using Domain.BondAggreagte.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common.Models;
public sealed class Bond
{
    public required string Ticker { get; set; }
    public required string Isin { get; set; }
    public required string Name { get; set; }
    public required decimal NominalIncome { get; set; }
    public required decimal Price { get; set; }
    public required DateTime MaturityDate { get; set; }
    public required DateTime? OfferDate { get; set; }
    public required int? Rating { get; set; }
    public required List<Coupon> Coupons { get; set; }
}
