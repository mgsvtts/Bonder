using Domain.UserAggregate.Abstractions.Repositories;
using Domain.UserAggregate.Entities;
using Mediator;

namespace Application.Queries.GetPortfolios;

public sealed class GetPortfoliosQueryHandler : IQueryHandler<GetPortfoliosQuery, IEnumerable<Portfolio>>
{
    private readonly IUserRepository _userRepository;

    public GetPortfoliosQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async ValueTask<IEnumerable<Portfolio>> Handle(GetPortfoliosQuery request, CancellationToken token)
    {
        var user = await _userRepository.GetOrCreateByIdAsync(request.UserId, token);

        return user.Portfolios;
    }
}