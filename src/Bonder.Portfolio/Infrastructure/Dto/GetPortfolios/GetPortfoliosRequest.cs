using Infrastructure.Dto.GetAccounts;

namespace Infrastructure.Dto.GetPortfolios;

public readonly record struct GetPortfoliosRequest(TinkoffAccount Account, string Token);