using Domain.UserAggregate.ValueObjects.Operations;
using LinqToDB.Mapping;

namespace Infrastructure.Common.Models;

[Table("operations")]
public sealed class Operation
{
    [Column("portfolio_id")]
    public Guid PortfolioId { get; set; }

    [Column("type")]
    public OperationType Type { get; set; }

    [Column("state")]
    public OperationState State { get; set; }

    [Column("date")]
    public DateTime Date { get; set; }

    [Column("payout")]
    public decimal Payout { get; set; }
}