using Application.Common.Abstractions;
using Domain.UserAggregate;
using Domain.UserAggregate.Abstractions.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AttachTinkoffToken;
public class AttachTinkoffTokenCommandHandler : IRequestHandler<AttachTinkoffTokenCommand, User>
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly ITinkoffHttpClient _httpClient;

    public AttachTinkoffTokenCommandHandler(IPortfolioRepository portfolioRepository, ITinkoffHttpClient httpClient)
    {
        _portfolioRepository = portfolioRepository;
        _httpClient = httpClient;
    }

    public async Task<User> Handle(AttachTinkoffTokenCommand request, CancellationToken cancellationToken)
    {
        var portfolios = await _httpClient.GetPortfoliosAsync(request.Token, cancellationToken);


        return await _portfolioRepository.AttachToken(request.UserName, request.Token, cancellationToken);
    }
}
