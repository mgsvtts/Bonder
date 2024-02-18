using Domain.BondAggreagte.Abstractions.Dto;

namespace Presentation.Controllers.BondController.Calculate.Response;
public readonly record struct CalculateResponse(IEnumerable<CalculatedBondResponse> CalculatedBonds,
                                                IEnumerable<PriceBondResponse> PriceSortedBonds,
                                                IEnumerable<IncomeBondResponse> CouponIncomeSortedBonds,
                                                IEnumerable<IncomeBondResponse> NominalIncomeSortedBonds,
                                                IEnumerable<IncomeBondResponse> FullIncomeSortedBonds,
                                                IEnumerable<CreditRatingBondResponse> CreditRatingSortedBonds,
                                                PageInfo PageInfo);