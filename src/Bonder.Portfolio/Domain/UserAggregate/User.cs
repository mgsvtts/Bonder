using Ardalis.GuardClauses;
using Domain.UserAggregate.ValueObjects;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Shared.Domain.Common.Models;

namespace Domain.UserAggregate;

public class User : AggregateRoot<UserName>
{
    private readonly List<Portfolio> _portfolios = [];

    public string TinkoffToken { get; private set; }
    public IReadOnlyList<Portfolio> Portfolios => _portfolios.AsReadOnly();

    public User(UserName userName, string tinkoffToken, IEnumerable<Portfolio>? portfolios = null) : base(userName)
    {
        TinkoffToken = Guard.Against.NullOrEmpty(tinkoffToken);
        if (portfolios is not null)
        {
            _portfolios = portfolios.ToList();
        }
    }
}