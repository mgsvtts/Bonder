using MediatR;

namespace Application.Commands.Register;

public sealed record RegisterCommand(string UserName, string Password) : IRequest;