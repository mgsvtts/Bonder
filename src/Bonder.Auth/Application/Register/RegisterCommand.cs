using MediatR;

namespace Application.Register;

public sealed record RegisterCommand(string UserName, string Password) : IRequest;