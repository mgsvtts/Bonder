using Domain.UserAggregate.ValueObjects;
using MediatR;

namespace Application.Login;

public record LoginCommand(UserName UserName, string Password) : IRequest<Tokens>;