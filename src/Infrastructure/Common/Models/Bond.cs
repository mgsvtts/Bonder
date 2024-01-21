using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    [Column("nominal_percent")]
    public required decimal NominalPercent { get; set; }

    [Column("price_percent")]
    public required decimal PricePercent { get; set; }

    [Column("original_price")]
    public required decimal OriginalPrice { get; set; }

    [Column("original_nominal")]
    public required decimal OriginalNominal { get; set; }

    [Column("maturity_date")]
    public required DateTime? MaturityDate { get; set; }

    [Column("offer_date")]
    public required DateTime? OfferDate { get; set; }

    [Column("rating")]
    public required int? Rating { get; set; }

    public List<Coupon> Coupons { get; set; }
}
