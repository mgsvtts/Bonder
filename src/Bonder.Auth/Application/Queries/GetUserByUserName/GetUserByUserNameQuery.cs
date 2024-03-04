using Domain.UserAggregate;
using Domain.UserAggregate.ValueObjects;
using Mediator;

namespace Application.Queries.GetUserByUserName;
public sealed record GetUserByUserNameQuery(UserName UserName) : IQuery<User>;