using Domain.UserAggregate.ValueObjects.Operations;
using Domain.UserAggregate.ValueObjects.Portfolios;

namespace Domain.UserAggregate.Abstractions.Repositories;
public interface IPortfolioRepository
{
    Task AddOperation(PortfolioId portfolioId, Operation operation, CancellationToken token);
}