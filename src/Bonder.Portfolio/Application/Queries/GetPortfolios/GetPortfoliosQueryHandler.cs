using Domain.UserAggregate.Abstractions.Repositories;
using Domain.UserAggregate.Entities;
using Mediator;

namespace Application.Queries.GetPortfolios;

public sealed class GetPortfoliosQueryHandler : IQueryHandler<GetPortfoliosQuery, IEnumerable<Portfolio>>
{
    private readonly IUserRepository _portfolioRepository;

    public GetPortfoliosQueryHandler(IUserRepository portfolioRepository)
    {
        _portfolioRepository = portfolioRepository;
    }

    public async ValueTask<IEnumerable<Portfolio>> Handle(GetPortfoliosQuery request, CancellationToken token)
    {
        var user = await _portfolioRepository.GetByIdAsync(request.UserId, token);

        return user.Portfolios;
    }
}