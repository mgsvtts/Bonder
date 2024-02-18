using Application.Common.Abstractions;
using Bonder.Auth;
using Domain.UserAggregate.Abstractions.Repositories;
using MediatR;

namespace Application.AttachTinkoffToken;

public class AttachTinkoffTokenCommandHandler : IRequestHandler<AttachTinkoffTokenCommand>
{
    private readonly ITinkoffHttpClient _httpClient;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly UserService.UserServiceClient _grpcClient;

    public AttachTinkoffTokenCommandHandler(IPortfolioRepository portfolioRepository, ITinkoffHttpClient httpClient, UserService.UserServiceClient grpcClient)
    {
        _portfolioRepository = portfolioRepository;
        _httpClient = httpClient;
        _grpcClient = grpcClient;
    }

    public async Task Handle(AttachTinkoffTokenCommand request, CancellationToken cancellationToken)
    {
        await ValidateUserAndTokenAsync(request, cancellationToken);

        await _portfolioRepository.AttachToken(request.UserName, request.Token, cancellationToken);
    }

    private Task ValidateUserAndTokenAsync(AttachTinkoffTokenCommand request, CancellationToken cancellationToken)
    {
        var portfoliosTask = _httpClient.GetPortfoliosAsync(request.Token, cancellationToken);

        var userTask = _grpcClient.GetUserAsync(new GetUserRequest
        {
            UserName = request.UserName.Name
        }, cancellationToken: cancellationToken);

        return Task.WhenAll(portfoliosTask, userTask.ResponseAsync);
    }
}