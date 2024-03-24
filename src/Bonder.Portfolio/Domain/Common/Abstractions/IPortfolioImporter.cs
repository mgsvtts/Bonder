using Domain.Common.Abstractions.Dto;
using Domain.UserAggregate;
using Domain.UserAggregate.Entities;

namespace Domain.Common.Abstractions;
public interface IPortfolioImporter
{
    Task<Portfolio> ImportAsync(AddPortfoliosToUserRequest request, CancellationToken token);
}