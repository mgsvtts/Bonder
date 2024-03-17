using Domain.Common.Abstractions;
using Domain.UserAggregate.Abstractions.Repositories;
using Mediator;

namespace Application.Commands.AttachTinkoffToken;

public sealed class RefreshPortfolioCommandHandler : ICommandHandler<RefreshPortfolioCommand>
{
    private readonly IUserBuilder _userBuilder;
    private readonly IUserRepository _userRepository;

    public RefreshPortfolioCommandHandler(IUserRepository portfolioRepository, IUserBuilder userBuilder)
    {
        _userRepository = portfolioRepository;
        _userBuilder = userBuilder;
    }

    public async ValueTask<Unit> Handle(RefreshPortfolioCommand request, CancellationToken token)
    {
        var user = await _userBuilder.BuildAsync(request.UserId, request.Token, token);

        await _userRepository.SaveAsync(user, token);

        return Unit.Value;
    }
}