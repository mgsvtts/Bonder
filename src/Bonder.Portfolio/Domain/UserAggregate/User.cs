using Domain.UserAggregate.Entities;
using Domain.UserAggregate.ValueObjects.Operations;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Domain.UserAggregate.ValueObjects.Users;
using Shared.Domain.Common.Models;

namespace Domain.UserAggregate;

public sealed class User : AggregateRoot<UserId>
{
    private readonly List<Portfolio> _portfolios = [];

    public TinkoffToken? Token { get; private set; }
    public IReadOnlyList<Portfolio> Portfolios => _portfolios.AsReadOnly();

    public User(UserId userId, TinkoffToken? token = null, IEnumerable<Portfolio>? portfolios = null) : base(userId)
    {
        Token = token;
        _portfolios = portfolios is not null ? portfolios.ToList() : _portfolios;
    }

    public User AddPortfolio(Portfolio portfolio)
    {
        _portfolios.Add(portfolio);

        return this;
    }
}