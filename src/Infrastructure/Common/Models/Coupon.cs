﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common.Models;

[Table("coupons")]
public sealed class Coupon
{
    [Key]
    [Column("id")]
    public required string Id { get; set; }

    [ForeignKey(nameof(Bond.Id))]
    [Column("bond_id")]
    public string BondId { get; set; }

    [Column("payment_date")]
    public required DateTime PaymentDate { get; set; }

    [Column("payout")]
    public required decimal Payout { get; set; }

    [Column("dividend_cut_off_date")]
    public required DateTime DividendCutOffDate { get; set; }

    [Column("is_floating")]
    public required bool IsFloating { get; set; }
}
