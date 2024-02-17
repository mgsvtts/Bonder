using MediatR;

namespace Application.Register;

public record RegisterCommand(string UserName, string Password, string Email) : IRequest;