namespace Presentation.Controllers.BondController.Calculate;
public sealed record CalculateJsonResponse(IEnumerable<CalculatedBondResponse> CalculatedBonds,
                                           IEnumerable<PriceBondResponse> PriceSortedBonds,
                                           IEnumerable<CouponeIncomeBondResponse> CouponIncomeSortedBonds,
                                           IEnumerable<IncomeBondResponse> IncomeSortedBonds);

public sealed record CalculatedBondResponse(string Ticker, string Uid, string Figi, string Name, int Priority);

public sealed record PriceBondResponse(string Ticker, string Uid, string Figi, string Name, decimal Price);

public sealed record CouponeIncomeBondResponse(string Ticker, string Uid, string Figi, string Name, decimal YearCouponIncome);

public sealed record IncomeBondResponse(string Ticker, string Uid, string Figi, string Name, decimal Income);