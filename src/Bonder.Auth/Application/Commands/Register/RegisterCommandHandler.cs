using Domain.UserAggregate;
using Domain.UserAggregate.Repositories;
using Domain.UserAggregate.ValueObjects;
using MediatR;

namespace Application.Commands.Register;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand>
{
    private readonly IUserRepository _userRepository;

    public RegisterCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new User(new UserId(Guid.NewGuid()), new UserName(request.UserName));

        await _userRepository.RegisterAsync(user, request.Password);
    }
}