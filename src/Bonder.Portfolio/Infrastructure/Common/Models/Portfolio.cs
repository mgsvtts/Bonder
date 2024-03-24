using Domain.UserAggregate.ValueObjects.Portfolios;
using LinqToDB.Mapping;

namespace Infrastructure.Common.Models;

[Table("portfolios")]
public sealed class Portfolio
{
    [PrimaryKey]
    [Column("id")]
    public required Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Association(ThisKey = nameof(UserId), OtherKey = nameof(User.Id))]
    public User User { get; set; }

    [Column("account_id")]
    public required string? AccountId { get; set; }

    [Column("name")]
    public required string Name { get; set; }

    [Column("total_bond_price")]
    public required decimal TotalBondPrice { get; set; }

    [Column("total_portfolio_price")]
    public required decimal TotalPortfolioPrice { get; set; }

    [Column("type")]
    public required PortfolioType Type { get; set; }

    [Column("broker_type")]
    public required BrokerType BrokerType { get; set; }

    [Association(ThisKey = nameof(Id), OtherKey = nameof(PortfolioBonds.PortfolioId))]
    public List<PortfolioBonds> Bonds { get; set; }

    [Association(ThisKey = nameof(Id), OtherKey = nameof(Operation.PortfolioId))]
    public List<Operation> Operations { get; set; }
}