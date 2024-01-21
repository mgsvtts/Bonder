using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Common.Models;

[Table("coupons")]
public sealed class Coupon
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [ForeignKey(nameof(Bond.Id))]
    [Column("bond_id")]
    public Guid BondId { get; set; }

    [Column("payment_date")]
    public DateTime PaymentDate { get; set; }

    [Column("payout")]
    public decimal Payout { get; set; }

    [Column("dividend_cut_off_date")]
    public DateTime DividendCutOffDate { get; set; }

    [Column("is_floating")]
    public bool IsFloating { get; set; }
}
