using Domain.BondAggreagte.Dto;

namespace Presentation.Controllers.BondController.Calculate.Request;
public sealed record CalculateRequest(CalculateRequestIncome Options, IEnumerable<string> Tickers);

public sealed record CalculateRequestIncome(DateIntervalType Type,
                                            DateOnly? TillDate,
                                            bool ConsiderDividendCutOffDate = true);
