using Domain.UserAggregate.ValueObjects.Users;
using Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.GetStats;
public sealed record GetStatsQuery(StatType Type, Guid Id, UserId CurrentUserId, DateTime DateFrom, DateTime DateTo) : IQuery<GetStatsResult>;

public enum StatType
{
    User,
    Portfolio
}