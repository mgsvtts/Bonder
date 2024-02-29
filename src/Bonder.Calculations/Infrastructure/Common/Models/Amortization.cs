using LinqToDB.Mapping;

namespace Infrastructure.Common.Models;

[Table("amortizations")]
public sealed class Amortization
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

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}