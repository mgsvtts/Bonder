using LinqToDB.Mapping;

namespace Infrastructure.Common.Models;

[Table("bonds")]
public sealed class Bond
{
    [PrimaryKey]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("ticker")]
    public string Ticker { get; set; }

    [Column("isin")]
    public string Isin { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("price_percent")]
    public decimal PricePercent { get; set; }

    [Column("absolute_price")]
    public decimal AbsolutePrice { get; set; }

    [Column("absolute_nominal")]
    public decimal AbsoluteNominal { get; set; }

    [Column("maturity_date")]
    public DateOnly? MaturityDate { get; set; }

    [Column("offer_date")]
    public DateOnly? OfferDate { get; set; }

    [Column("rating")]
    public int? Rating { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Association(ThisKey = nameof(Id), OtherKey = nameof(Coupon.BondId))]
    public List<Coupon> Coupons { get; set; }

    [Association(ThisKey = nameof(Id), OtherKey = nameof(Amortization.BondId))]
    public List<Amortization> Amortizations { get; set; }
}