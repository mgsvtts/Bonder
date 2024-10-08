﻿using Application.Commands.Portfolios.RefreshPortfolio;
using Application.Commands.Users.DeleteUser;
using Bonder.Portfolio.Grpc;
using Domain.UserAggregate.ValueObjects.Users;
using Grpc.Core;
using Mediator;
using Void = Bonder.Portfolio.Grpc.Void;

namespace Presentation.Grpc;

public sealed class PortfolioServiceImpl : PortfolioService.PortfolioServiceBase
{
    private readonly ISender _sender;

    public PortfolioServiceImpl(ISender sender)
    {
        _sender = sender;
    }

    public override async Task<Void> DeleteUser(DeleteUserRequest request, ServerCallContext context)
    {
        await _sender.Send(new DeleteUserCommand(new UserId(Guid.Parse(request.UserName))));

        return new Void();
    }

    public override async Task<Void> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        await _sender.Send(new RefreshPortfolioCommand(new UserId(request.UserId), new TinkoffToken(request.Token)));

        return new Void();  
    }
}