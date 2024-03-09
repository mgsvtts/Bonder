using Application.Common.Abstractions;
using Domain.UserAggregate.Abstractions.Repositories;
using Mediator;

namespace Application.AttachTinkoffToken;

public sealed class AttachTinkoffTokenCommandHandler : ICommandHandler<AttachTinkoffTokenCommand>
{
    private readonly IUserBuilder _userBuilder;
    private readonly IUserRepository _userRepository;

    public AttachTinkoffTokenCommandHandler(IUserRepository portfolioRepository, IUserBuilder userBuilder)
    {
        _userRepository = portfolioRepository;
        _userBuilder = userBuilder;
    }

    public async ValueTask<Unit> Handle(AttachTinkoffTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userBuilder.BuildAsync(request.UserId, request.Token, cancellationToken);

        await _userRepository.SaveAsync(user, cancellationToken);

        return Unit.Value;
    }
}