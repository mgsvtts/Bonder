using Domain.UserAggregate.ValueObjects.Portfolios;
using LinqToDB.Mapping;

namespace Infrastructure.Common.Models;

[Table("portfolios")]
public sealed class Portfolio
{
    [PrimaryKey]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_name")]
    public string UserName { get; set; }

    [Association(ThisKey = nameof(UserName), OtherKey = nameof(User.UserName))]
    public User User { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("total_bond_price")]
    public decimal TotalBondPrice { get; set; }

    [Column("type")]
    public PortfolioType Type { get; set; }

    [Column("status")]
    public PortfolioStatus Status { get; set; }

    [Association(ThisKey = nameof(Id), OtherKey = nameof(PortfolioBonds.PortfolioId))]
    public List<PortfolioBonds> Bonds { get; set; }
}