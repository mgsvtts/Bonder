using LinqToDB.Mapping;

namespace Infrastructure.Common.Models;

[Table("trades")]
public sealed class Trade
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("operation_id")]
    public Guid OperationId { get; set; }

    [Column("date")]
    public DateTime Date { get; set; }

    [Column("quantity")]
    public decimal Quantity { get; set; }

    [Column("price")]
    public decimal Price { get; set; }
}