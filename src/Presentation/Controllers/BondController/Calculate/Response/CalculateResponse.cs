namespace Presentation.Controllers.BondController.Calculate.Response;
public sealed record CalculateResponse(IEnumerable<CalculatedBondResponse> CalculatedBonds,
                                       IEnumerable<PriceBondResponse> PriceSortedBonds,
                                       IEnumerable<IncomeBondResponse> CouponIncomeSortedBonds,
                                       IEnumerable<IncomeBondResponse> NominalIncomeSortedBonds,
                                       IEnumerable<IncomeBondResponse> FullIncomeSortedBonds);