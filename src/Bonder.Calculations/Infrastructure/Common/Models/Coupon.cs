using LinqToDB.Mapping;

namespace Infrastructure.Common.Models;

[Table("coupons")]
public sealed class Coupon
{
    [PrimaryKey]
    [Column("id")]
    public required Guid Id { get; set; }

    [Column("bond_id")]
    public Guid BondId { get; set; }

    [Association(ThisKey = nameof(BondId), OtherKey = nameof(Bond.Id))]
    public Bond Bond { get; set; }

    [Column("payment_date")]
    public required DateOnly PaymentDate { get; set; }

    [Column("payout")]
    public required decimal Payout { get; set; }

    [Column("dividend_cut_off_date")]
    public required DateOnly? DividendCutOffDate { get; set; }

    [Column("is_floating")]
    public required bool IsFloating { get; set; }

    [Column("created_at")]
    public required DateTime CreatedAt { get; set; }
}