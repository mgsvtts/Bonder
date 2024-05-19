using Application.Commands.Operations.Create;
using Domain.UserAggregate.ValueObjects.Trades;

namespace Domain.UserAggregate.ValueObjects.Operations;

public readonly record struct Operation
{
    private readonly List<Trade> _trades = [];
    public OperationName Name { get; }
    public string? Description { get; }
    public OperationType Type { get; }
    public OperationState State { get; }
    public DateTime Date { get; }
    public decimal Payout { get; }
    public decimal Price { get; }
    public decimal Commisison { get; }
    public InstrumentType InstrumentType { get; }
    public Guid? InstrumentId { get; }
    public int Quantity { get; }
    public int RestQuantity { get; }
    public IReadOnlyList<Trade> Trades => _trades.AsReadOnly();

    public Operation(OperationName name,
                     OperationType type,
                     OperationState state,
                     DateTime date,
                     decimal payout,
                     decimal price,
                     decimal commisison,
                     InstrumentType instrumentType,
                     int quantity,
                     int restQuantity = 0,
                     Guid? instrumentId = null,
                     IEnumerable<Trade>? trades = null,
                     string? description = null)
    {
        Name = name;
        Description = description;
        Type = type;
        State = state;
        Date = date;
        InstrumentType = instrumentType;
        InstrumentId = instrumentId;
        Quantity = quantity;
        RestQuantity = restQuantity;
        Price = price < 0 ? Math.Abs(price) : price;
        Payout = payout < 0 ? Math.Abs(payout) : payout;
        Commisison = commisison < 0 ? Math.Abs(commisison) : commisison;

        _trades = trades is not null ? _trades.ToList() : _trades;
    }
}