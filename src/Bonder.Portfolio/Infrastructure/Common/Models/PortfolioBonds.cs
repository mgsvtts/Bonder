using LinqToDB.Mapping;

namespace Infrastructure.Common.Models;

[Table("portfolio_bonds")]
public sealed class PortfolioBonds
{
    [Column("portfolio_id")]
    public required Guid PortfolioId { get; set; }

    [Column("bond_id")]
    public required Guid BondId { get; set; }

    [Column("count")]
    public required decimal Count { get; set; }
}