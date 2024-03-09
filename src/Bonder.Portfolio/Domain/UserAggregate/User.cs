using Ardalis.GuardClauses;
using Domain.UserAggregate.ValueObjects;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Shared.Domain.Common.Models;

namespace Domain.UserAggregate;

public sealed class User : AggregateRoot<UserId>
{
    private readonly List<Portfolio> _portfolios = [];

    public string TinkoffToken { get; private set; }
    public IReadOnlyList<Portfolio> Portfolios => _portfolios.AsReadOnly();

    public User(UserId userId, string tinkoffToken, IEnumerable<Portfolio>? portfolios = null) : base(userId)
    {
        TinkoffToken = Guard.Against.NullOrEmpty(tinkoffToken);
        _portfolios = portfolios is not null ? portfolios.ToList() : _portfolios;
    }

    public User AddPortfolio(decimal bondsPrice, BrokerType brokerType, IEnumerable<Bond> bonds)
    {
        var portfolio = new Portfolio
        (
            bondsPrice,
            $"{brokerType}-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}",
            PortfolioType.Exported,
            brokerType,
            bonds
        );

        _portfolios.Add(portfolio);

        return this;
    }
}