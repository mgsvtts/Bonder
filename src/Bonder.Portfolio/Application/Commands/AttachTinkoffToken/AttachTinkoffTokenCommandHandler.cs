using Domain.Common.Abstractions;
using Domain.UserAggregate.Abstractions.Repositories;
using Mediator;

namespace Application.Commands.AttachTinkoffToken;

public sealed class AttachTinkoffTokenCommandHandler : ICommandHandler<AttachTinkoffTokenCommand>
{
    private readonly IUserBuilder _userBuilder;
    private readonly IUserRepository _userRepository;

    public AttachTinkoffTokenCommandHandler(IUserRepository portfolioRepository, IUserBuilder userBuilder)
    {
        _userRepository = portfolioRepository;
        _userBuilder = userBuilder;
    }

    public async ValueTask<Unit> Handle(AttachTinkoffTokenCommand request, CancellationToken token)
    {
        var user = await _userBuilder.BuildAsync(request.UserId, request.Token, token);

        await _userRepository.SaveAsync(user, token);

        return Unit.Value;
    }
}