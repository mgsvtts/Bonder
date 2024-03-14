using Bonder.Portfolio.Grpc;
using Domain.UserAggregate.Repositories;
using Mediator;

namespace Application.Commands.DeleteUser;

public sealed class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly PortfolioService.PortfolioServiceClient _grpcClient;

    public DeleteUserCommandHandler(IUserRepository userRepository, PortfolioService.PortfolioServiceClient grpcClient)
    {
        _userRepository = userRepository;
        _grpcClient = grpcClient;
    }

    public async ValueTask<Unit> Handle(DeleteUserCommand request, CancellationToken token)
    {
        var deletedUser = await _userRepository.DeleteAsync(request.UserId, token);
        await _grpcClient.DeleteUserAsync(new DeleteUserRequest
        {
            UserName = deletedUser.UserName.Name
        }, cancellationToken: token);

        return Unit.Value;
    }
}