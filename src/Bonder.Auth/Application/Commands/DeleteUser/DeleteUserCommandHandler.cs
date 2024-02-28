using Bonder.Portfolio.Grpc;
using Domain.UserAggregate.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.DeleteUser;
public sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly PortfolioService.PortfolioServiceClient _grpcClient;

    public DeleteUserCommandHandler(IUserRepository userRepository, PortfolioService.PortfolioServiceClient grpcClient)
    {
        _userRepository = userRepository;
        _grpcClient = grpcClient;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var deletedUser = await _userRepository.DeleteAsync(request.UserId, cancellationToken);
        await _grpcClient.DeleteUserAsync(new DeleteUserRequest
        {
           UserName = deletedUser.UserName.Name 
        }, cancellationToken: cancellationToken);
    }
}
