using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Common.Models;

[Table("bonds")]
public sealed class Bond
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("ticker")]
    public string Ticker { get; set; }

    [Column("isin")]
    public string Isin { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("nominal_income")]
    public decimal NominalIncome { get; set; }

    [Column("price")]
    public decimal Price { get; set; }

    [Column("maturity_date")]
    public DateTime? MaturityDate { get; set; }

    [Column("offer_date")]
    public DateTime? OfferDate { get; set; }

    [Column("rating")]
    public int? Rating { get; set; }

    public List<Coupon> Coupons { get; set; }
}
