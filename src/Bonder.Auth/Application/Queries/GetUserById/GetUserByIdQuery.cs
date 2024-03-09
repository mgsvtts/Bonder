using Domain.UserAggregate;
using Domain.UserAggregate.ValueObjects;
using Mediator;

namespace Application.Queries.GetUserById;
public sealed record GetUserByIdQuery(UserId Id) : IQuery<User?>;