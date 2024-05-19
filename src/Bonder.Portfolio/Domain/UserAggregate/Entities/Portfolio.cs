using Domain.UserAggregate.ValueObjects.Operations;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Shared.Domain.Common.Models;

namespace Domain.UserAggregate.Entities;
public sealed class Portfolio : Entity<PortfolioId>
{
    private readonly List<Bond> _bonds = [];
    private readonly List<Operation> _operations = [];

    public AccountId? AccountId { get; }
    public Totals Totals { get; }
    public PortfolioName Name { get; }
    public PortfolioType Type { get; }
    public BrokerType BrokerType { get; }
    public IReadOnlyList<Bond> Bonds => _bonds.AsReadOnly();
    public IReadOnlyList<Operation> Operations => _operations.AsReadOnly();

    public Portfolio(PortfolioId id,
                     Totals totals,
                     PortfolioName name,
                     PortfolioType type,
                     BrokerType brokerType,
                     IEnumerable<Bond>? bonds,
                     IEnumerable<Operation>? operations = null,
                     AccountId? accountId = null) : base(id)
    {
        Name = name;
        Type = type;
        AccountId = accountId;
        BrokerType = brokerType;
        Totals = totals;

        _bonds = bonds is not null ? bonds.ToList() : _bonds;
        _operations = operations is not null ? operations.ToList() : _operations;
    }

    public Portfolio AddOperations(IEnumerable<Operation> operations)
    {
        if (operations is null || !operations.Any())
        {
            return this;
        }

        _operations.AddRange(operations);

        return this;
    }
}