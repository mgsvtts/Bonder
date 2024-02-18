using Domain.UserAggregate.ValueObjects;
using MediatR;

namespace Application.AttachTinkoffToken;
public sealed record AttachTinkoffTokenCommand(UserName UserName, string Token) : IRequest;