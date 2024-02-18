using Ardalis.GuardClauses;
using Domain.UserAggregate.ValueObjects;
using Shared.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UserAggregate;
public class User : AggregateRoot<UserName>
{
    public string TinkoffToken { get; private set; }

    public User(UserName id, string tinkoffToken) : base(id)
    {
        TinkoffToken = Guard.Against.NullOrEmpty(tinkoffToken);
    }
}
