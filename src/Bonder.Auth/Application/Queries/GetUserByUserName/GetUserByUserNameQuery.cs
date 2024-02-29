using Domain.UserAggregate;
using Domain.UserAggregate.ValueObjects;
using MediatR;

namespace Application.Queries.GetUserByUserName;
public sealed record GetUserByUserNameQuery(UserName UserName) : IRequest<User>;