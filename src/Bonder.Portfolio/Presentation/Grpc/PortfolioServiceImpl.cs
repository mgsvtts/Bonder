using Application.DeleteUser;
using Bonder.Auth.Grpc;
using Bonder.Portfolio.Grpc;
using Domain.UserAggregate.ValueObjects;
using Grpc.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Grpc;
public sealed class PortfolioServiceImpl : PortfolioService.PortfolioServiceBase
{
    private readonly ISender _sender;

    public PortfolioServiceImpl(ISender sender)
    {
        _sender = sender;
    }

    public override async Task<Bonder.Portfolio.Grpc.Void> DeleteUser(DeleteUserRequest request, ServerCallContext context)
    {
        await _sender.Send(new DeleteUserCommand(new UserName(request.UserName)));

        return new Bonder.Portfolio.Grpc.Void();
    }
}
