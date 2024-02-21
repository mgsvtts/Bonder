using Application.Common.Abstractions;
using Domain.UserAggregate.Abstractions.Repositories;
using MediatR;

namespace Application.AttachTinkoffToken;

public class AttachTinkoffTokenCommandHandler : IRequestHandler<AttachTinkoffTokenCommand>
{
    private readonly IUserBuilder _userBuilder;
    private readonly IPortfolioRepository _portfolioRepository;

    public AttachTinkoffTokenCommandHandler(IPortfolioRepository portfolioRepository, IUserBuilder userBuilder)
    {
        _portfolioRepository = portfolioRepository;
        _userBuilder = userBuilder;
    }

    public async Task Handle(AttachTinkoffTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userBuilder.BuildAsync(request.UserName, request.Token, cancellationToken);

        await _portfolioRepository.AttachToken(user.Identity, user.TinkoffToken, cancellationToken);
    }
}