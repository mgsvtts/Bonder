using Ardalis.GuardClauses;
using Domain.UserAggregate.Entities;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Domain.UserAggregate.ValueObjects.Users;
using Shared.Domain.Common.Models;

namespace Domain.UserAggregate;

public sealed class User : AggregateRoot<UserId>
{
    private readonly List<Portfolio> _portfolios = [];

    public TinkoffToken Token { get; private set; }
    public IReadOnlyList<Portfolio> Portfolios => _portfolios.AsReadOnly();

    public User(UserId userId, TinkoffToken token, IEnumerable<Portfolio>? portfolios = null) : base(userId)
    {
        Token = token;
        _portfolios = portfolios is not null ? portfolios.ToList() : _portfolios;
    }

    public User AddImportedPortfolio(decimal bondsPrice, BrokerType brokerType, IEnumerable<Bond>? bonds, string? name =null)
    {
        _portfolios.Add(new Portfolio
        (
            new PortfolioId(Guid.NewGuid()),
            bondsPrice,
            name ?? $"{brokerType}-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}",
            PortfolioType.Exported,
            brokerType,
            bonds
        ));

        return this;
    }
}
