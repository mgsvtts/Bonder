namespace Presentation.Controllers.BondController.CalculateJson;
public sealed record CalculateJsonResponse(IEnumerable<CalculatedBondResponse> CalculatedBonds,
                                           IEnumerable<PriceBondResponse> PriceSortedBonds,
                                           IEnumerable<CouponeIncomeBondResponse> CouponIncomeSortedBonds,
                                           IEnumerable<IncomeBondResponse> IncomeSortedBonds);

public sealed record CalculatedBondResponse(string Ticker, string Name, int Priority);

public sealed record PriceBondResponse(string Ticker, string Name, decimal Price);

public sealed record CouponeIncomeBondResponse(string Ticker, string Name, decimal YearCouponIncome);

public sealed record IncomeBondResponse(string Ticker, string Name, decimal Income);