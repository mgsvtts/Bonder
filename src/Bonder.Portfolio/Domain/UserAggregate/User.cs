using Ardalis.GuardClauses;
using Domain.UserAggregate.ValueObjects;
using Shared.Domain.Common.Models;

namespace Domain.UserAggregate;

public class User : AggregateRoot<UserName>
{
    public string TinkoffToken { get; private set; }

    public User(UserName userName, string tinkoffToken) : base(userName)
    {
        TinkoffToken = Guard.Against.NullOrEmpty(tinkoffToken);
    }
}