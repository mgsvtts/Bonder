namespace Presentation.Controllers.BondController.Calculate.Response;
public sealed record CalculateResponse(IEnumerable<CalculatedBondResponse> CalculatedBonds,
                                       IEnumerable<PriceBondResponse> PriceSortedBonds,
                                       IEnumerable<CouponeIncomeBondResponse> CouponIncomeSortedBonds,
                                       IEnumerable<IncomeBondResponse> IncomeSortedBonds);