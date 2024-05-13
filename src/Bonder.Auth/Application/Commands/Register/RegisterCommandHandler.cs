using Domain.UserAggregate;
using Domain.UserAggregate.Repositories;
using Domain.UserAggregate.ValueObjects;
using Mediator;
using Shared.Domain.Common.ValueObjects;

namespace Application.Commands.Register;

public sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand>
{
    private readonly IUserRepository _userRepository;

    public RegisterCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async ValueTask<Unit> Handle(RegisterCommand request, CancellationToken token)
    {
        var user = new User(new UserId(Guid.NewGuid()), new ValidatedString(request.UserName));

        await _userRepository.RegisterAsync(user, request.Password);

        return Unit.Value;
    }
}