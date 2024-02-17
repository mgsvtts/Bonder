using LinqToDB.Mapping;

namespace Infrastructure.Common.Models;

[Table("coupons")]
public sealed class Coupon
{
    [PrimaryKey]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("bond_id")]
    public Guid BondId { get; set; }

    [Association(ThisKey = nameof(BondId), OtherKey = nameof(Bond.Id))]
    public Bond Bond { get; set; }

    [Column("payment_date")]
    public DateOnly PaymentDate { get; set; }

    [Column("payout")]
    public decimal Payout { get; set; }

    [Column("dividend_cut_off_date")]
    public DateOnly? DividendCutOffDate { get; set; }

    [Column("is_floating")]
    public bool IsFloating { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}