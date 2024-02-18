using Bonder.Auth;
using Domain.UserAggregate.Repositories;
using Domain.UserAggregate.ValueObjects;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Grpc;
public sealed class UserServiceImpl  : UserService.UserServiceBase
{
    private readonly IUserRepository _userRepository;

    public UserServiceImpl(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public override async Task<User> GetUser(GetUserRequest request, ServerCallContext context)
    {
        var user = await _userRepository.GetByUserNameAsync(new UserName(request.UserName), context.CancellationToken)
        ?? throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

        return new User
        {
            Id = user.Identity.ToString(),
            UserName = request.UserName,
            IsAdmin = user.IsAdmin
        };
    }
}
