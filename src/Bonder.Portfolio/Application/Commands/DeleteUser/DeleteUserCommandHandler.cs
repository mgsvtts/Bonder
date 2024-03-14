using Domain.UserAggregate.Abstractions.Repositories;
using Mediator;

namespace Application.Commands.DeleteUser;

public sealed class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async ValueTask<Unit> Handle(DeleteUserCommand request, CancellationToken token)
    {
        await _userRepository.DeleteAsync(request.UserName, token);

        return Unit.Value;
    }
}